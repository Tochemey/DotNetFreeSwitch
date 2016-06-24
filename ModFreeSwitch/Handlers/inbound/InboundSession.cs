using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Handlers.outbound;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundSession : IInboundListener {
        private IChannel _channel;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private ConnectedCall _connectedCall;

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

        public Task OnDisconnectNotice() {
            _logger.Warn("channel {0} disconnected", _channel.LocalAddress);
            return Task.CompletedTask;
        }

        public Task OnRudeRejection() {
            throw new NotImplementedException();
        }

        public async Task OnError(Exception exception) {
            if (exception is DecoderException) {
                _logger.Warn(
                    $"Encountered an issue during encoding: {exception}. shutting down...");
                await _channel.CloseAsync();
                return;
            }

            if (exception is SocketException) {
                _logger.Warn(
                    $"Encountered an issue on the channel: {exception}. shutting down...");
                await _channel.CloseAsync();
                return;
            }

            _logger.Error($"Encountered an issue : {exception}");
        }

        public Task OnConnected(ConnectedCall connectedInfo, IChannel  channel) {
            _connectedCall = connectedInfo;
            _channel = channel;
            return Task.CompletedTask;
        }

        public bool CanSend() {
            return _channel != null && _channel.Active;
        }

        public async Task<CommandReply> SendCommandAsync(BaseCommand command) {
            if (!CanSend()) return null;
            var handler = (InboundSessionHandler) _channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command, _channel);
            return reply;
        }

        public async Task<ApiResponse> SendApiAsync(ApiCommand apiCommand) {
            if (!CanSend()) return null;
            var handler = (InboundSessionHandler) _channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand, _channel);
            return response;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand) {
            if (!CanSend()) return Guid.Empty;
            var handler = (InboundSessionHandler) _channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand, _channel);
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