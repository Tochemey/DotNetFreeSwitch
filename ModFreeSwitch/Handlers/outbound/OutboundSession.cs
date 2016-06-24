using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.outbound {
    public class OutboundSession : IOutboundListener {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Bootstrap _bootstrap;
        private IChannel _channel;

        private MultithreadEventLoopGroup _eventLoopGroup;

        public OutboundSession(string address,
            int port,
            string password,
            TimeSpan connectionTimeout) {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = connectionTimeout;
        }

        public OutboundSession(string address,
            int port,
            string password) {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = new TimeSpan(0, 0, 0, 0, 1000);
        }

        public OutboundSession() : this("localhost", 8021, "ClueCon") {}

        public OutboundSession(TimeSpan timeout)
            : this("localhost", 8021, "ClueCon", timeout) {}

        public string Address { get; }
        public bool Authenticated { get; private set; }
        public TimeSpan ConnectionTimeout { get; }
        public string Password { get; }
        public int Port { get; }

        public async Task OnAuthentication() {
            await AuthenticateAsync();
        }

        public async Task OnDisconnectNotice() {
            _logger.Warn("channel {0} disconnected", _channel.RemoteAddress);
            await CleanUpAsync();
        }

        public async Task OnError(Exception exception) {
            // disconnect when we have encountered system related errors
            if (exception is DecoderException) {
                _logger.Warn(
                    $"Encountered an issue during encoding: {exception}. shutting down...");
                await DisconnectAsync();
                return;
            }

            if (exception is SocketException) {
                _logger.Warn(
                    $"Encountered an issue on the channel: {exception}. shutting down...");
                await DisconnectAsync();
                return;
            }

            _logger.Error($"Encountered an issue : {exception}");
        }

        public async Task OnEventReceived(EslEvent eslEvent) {
            var eventName = eslEvent.EventName;
            var eventType = Enumm.Parse<EslEventType>(eventName);
            var eslEventArgs = new EslEventArgs(eslEvent);
            AsyncEventHandler<EslEventArgs> handler = null;
            switch (eventType) {
                case EslEventType.BACKGROUND_JOB:
                    handler = OnBackgroundJob;
                    break;
                case EslEventType.CALL_UPDATE:
                    handler = OnCallUpdate;
                    break;
                case EslEventType.CHANNEL_BRIDGE:
                    handler = OnChannelBridge;
                    break;
                case EslEventType.CHANNEL_HANGUP:
                    handler = OnChannelHangup;
                    break;
                case EslEventType.CHANNEL_HANGUP_COMPLETE:
                    handler = OnChannelHangupComplete;
                    break;
                case EslEventType.CHANNEL_PROGRESS:
                    handler = OnChannelProgress;
                    break;
                case EslEventType.CHANNEL_PROGRESS_MEDIA:
                    handler = OnChannelProgressMedia;
                    break;
                case EslEventType.CHANNEL_EXECUTE:
                    handler = OnChannelExecute;
                    break;
                case EslEventType.CHANNEL_EXECUTE_COMPLETE:
                    handler = OnChannelExecuteComplete;
                    break;
                case EslEventType.CHANNEL_UNBRIDGE:
                    handler = OnChannelUnbridge;
                    break;
                case EslEventType.SESSION_HEARTBEAT:
                    handler = OnSessionHeartbeat;
                    break;
                case EslEventType.DTMF:
                    handler = OnDtmf;
                    break;
                case EslEventType.RECORD_STOP:
                    handler = OnRecordStop;
                    break;
                case EslEventType.CUSTOM:
                    handler = OnCustom;
                    break;
                case EslEventType.CHANNEL_STATE:
                    handler = OnChannelState;
                    break;
                case EslEventType.CHANNEL_ANSWER:
                    handler = OnChannelAnswer;
                    break;
                case EslEventType.CHANNEL_ORIGINATE:
                    handler = OnChannelOriginate;
                    break;
                case EslEventType.CHANNEL_PARK:
                    handler = OnChannelPark;
                    break;
                case EslEventType.CHANNEL_UNPARK:
                    handler = OnChannelUnPark;
                    break;
                default:
                    _logger.Debug("received unhandled freeSwitch event {0}", eslEvent);
                    handler = OnReceivedUnHandledEvent;
                    break;
            }
            if (handler != null) await handler(this, eslEventArgs);
        }

        public async Task OnRudeRejection() {
            _logger.Warn("channel {0} received rude/rejection", _channel.RemoteAddress);
            await CleanUpAsync();
        }

        public bool CanSend() {
            return Authenticated && IsActive();
        }

        public async Task CleanUpAsync() {
            if (_eventLoopGroup != null) await _eventLoopGroup.ShutdownGracefullyAsync();
        }

        public async Task ConnectAsync() {
            _logger.Info("connecting to freeSwitch mod_event_socket...");
            Initialize();
            _channel = await _bootstrap.ConnectAsync(Address, Port);
            _logger.Info("successfully connected to freeSwitch mod_event_socket.");
        }

        public async Task DisconnectAsync() {
            if (_channel != null) await _channel.CloseAsync();
            if (_eventLoopGroup != null) await _eventLoopGroup.ShutdownGracefullyAsync();
        }

        public bool IsActive() {
            return _channel != null && _channel.Active;
        }

        public async Task<ApiResponse> SendApiAsync(ApiCommand apiCommand) {
            if (!CanSend()) return null;
            var handler = (OutboundSessionHandler) _channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand, _channel);
            return response;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand) {
            if (!CanSend()) return Guid.Empty;
            var handler = (OutboundSessionHandler) _channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand, _channel);
        }

        public async Task<CommandReply> SendCommandAsync(BaseCommand command) {
            if (!CanSend()) return null;
            var handler = (OutboundSessionHandler) _channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command, _channel);
            return reply;
        }

        public async Task<bool> SubscribeAsync(string events) {
            if (!CanSend()) return false;
            var handler = (OutboundSessionHandler) _channel.Pipeline.Last();
            var command = new EventCommand(events);
            var reply = await handler.SendCommandAsync(command, _channel);
            return reply.IsOk;
        }

        protected async Task AuthenticateAsync() {
            var command = new AuthCommand(Password);
            var handler = (OutboundSessionHandler) _channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command, _channel);
            Authenticated = reply.IsOk;
        }

        protected void Initialize() {
            _eventLoopGroup = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();
            _bootstrap.Group(_eventLoopGroup);
            _bootstrap.Channel<TcpSocketChannel>();
            _bootstrap.Option(ChannelOption.SoLinger, 1);
            _bootstrap.Option(ChannelOption.TcpNodelay, true);
            _bootstrap.Option(ChannelOption.SoKeepalive, true);
            _bootstrap.Option(ChannelOption.SoReuseaddr, true);
            _bootstrap.Option(ChannelOption.ConnectTimeout, ConnectionTimeout);
            _bootstrap.Handler(new OutboundSessionInitializer(this));
        }

        #region FreeSwitch Events Handlers

        public event AsyncEventHandler<EslEventArgs> OnBackgroundJob;

        public event AsyncEventHandler<EslEventArgs> OnCallUpdate;

        public event AsyncEventHandler<EslEventArgs> OnChannelAnswer;

        public event AsyncEventHandler<EslEventArgs> OnChannelBridge;

        public event AsyncEventHandler<EslEventArgs> OnChannelExecute;

        public event AsyncEventHandler<EslEventArgs> OnChannelExecuteComplete;

        public event AsyncEventHandler<EslEventArgs> OnChannelHangup;

        public event AsyncEventHandler<EslEventArgs> OnChannelHangupComplete;

        public event AsyncEventHandler<EslEventArgs> OnChannelOriginate;

        public event AsyncEventHandler<EslEventArgs> OnChannelPark;

        public event AsyncEventHandler<EslEventArgs> OnChannelProgress;

        public event AsyncEventHandler<EslEventArgs> OnChannelProgressMedia;

        public event AsyncEventHandler<EslEventArgs> OnChannelState;

        public event AsyncEventHandler<EslEventArgs> OnChannelUnbridge;

        public event AsyncEventHandler<EslEventArgs> OnChannelUnPark;

        public event AsyncEventHandler<EslEventArgs> OnCustom;

        public event AsyncEventHandler<EslEventArgs> OnDtmf;

        public event AsyncEventHandler<EslEventArgs> OnReceivedUnHandledEvent;

        public event AsyncEventHandler<EslEventArgs> OnRecordStop;

        public event AsyncEventHandler<EslEventArgs> OnSessionHeartbeat;

        #endregion
    }
}