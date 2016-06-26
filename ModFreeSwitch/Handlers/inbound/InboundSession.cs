using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundSession : IInboundListener {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IChannel _channel;
        private ConnectedCall _connectedCall;

        public async Task OnConnected(ConnectedCall connectedInfo,
            IChannel channel) {
            _connectedCall = connectedInfo;
            _channel = channel;
            await ResumeAsync();
            await MyEventsAsync();
            await DivertEventsAsync(true);
            // perform other stuff with the connected call here
        }

        public Task OnDisconnectNotice() {
            _logger.Warn("channel {0} disconnected", _channel.LocalAddress);
            return Task.CompletedTask;
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
            if (_channel != null) await _channel.CloseAsync();
        }

        public async Task AnswerAsync() {
            await ExecuteAsync("answer", true);
        }

        public async Task BindDigitActionAsync(string command,
            bool eventLock = true) {
            await ExecuteAsync("bind_digit_action", command, eventLock);
        }

        public async Task BridgeAsync(string bridgeText,
            bool eventLock = false) {
            await ExecuteAsync("bridge", bridgeText, eventLock);
        }

        public bool CanHandleEvent(EslEvent @event) {
            return (@event.UniqueId == _connectedCall.UniqueId) &&
                   @event.CallerGuid == _connectedCall.CallerGuid;
        }

        public bool CanSend() {
            return _channel != null && _channel.Active;
        }

        public async Task DivertEventsAsync(bool flag) {
            var command = new DivertEventsCommand(flag);
            await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            string arguments,
            bool eventLock) {
            var command = new SendMsgCommand(_connectedCall.CallerGuid,
                SendMsgCommand.CALL_COMMAND,
                application,
                arguments,
                eventLock);
            return await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            string arguments,
            int loop,
            bool eventLock) {
            var command = new SendMsgCommand(_connectedCall.CallerGuid,
                SendMsgCommand.CALL_COMMAND,
                application,
                arguments,
                eventLock,
                loop);
            return await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application) {
            return await ExecuteAsync(application, string.Empty, false);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            bool eventLock) {
            return await ExecuteAsync(application, string.Empty, eventLock);
        }

        public async Task MyEventsAsync() {
            await SendCommandAsync(new MyEventsCommand(_connectedCall.CallerGuid));
        }

        public async Task PlayAsync(string audioFile,
            bool eventLock) {
            await ExecuteAsync("playback", audioFile, eventLock);
        }

        public async Task PreAnswerAsync() {
            await ExecuteAsync("pre_answer");
        }

        public async Task ResumeAsync() {
            await SendCommandAsync(new ResumeCommand());
        }

        public async Task RingReadyAsync() {
            await ExecuteAsync("ring_ready", true);
        }

        public async Task RingReadyAsync(bool eventLock) {
            await ExecuteAsync("ring_ready", eventLock);
        }

        public async Task SayAsync(string language,
            SayTypes type,
            SayMethods method,
            SayGenders gender,
            string text,
            int loop,
            bool eventLock) {
            await ExecuteAsync("say",
                language + " " + type + " " + method.ToString()
                    .Replace("_", "/") + " " + gender + " " + text,
                loop,
                eventLock);
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

        public async Task<CommandReply> SendCommandAsync(BaseCommand command) {
            if (!CanSend()) return null;
            var handler = (InboundSessionHandler) _channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command, _channel);
            return reply;
        }

        public async Task SetAsync(string variableName,
            string variableValue) {
            await ExecuteAsync("set", variableName + "=" + variableValue, false);
        }

        public async Task SetAsync(string variableName,
            string variableValue,
            bool eventLock) {
            await ExecuteAsync("set", variableName + "=" + variableValue, eventLock);
        }

        public async Task SleepAsync(int millisecond,
            bool eventLock = false) {
            await ExecuteAsync("sleep", Convert.ToString(millisecond), eventLock);
        }

        public async Task SpeakAsync(string engine,
            string voice,
            string text,
            string timerName = null,
            int loop = 1,
            bool eventLock = false) {
            await
                ExecuteAsync("speak",
                    engine + "|" + voice + "|" + text +
                    (!string.IsNullOrEmpty(timerName) ? "|" + timerName : ""),
                    loop,
                    eventLock);
        }

        public async Task SpeakAsync(string engine,
            int loop = 1,
            bool eventLock = false) {
            await ExecuteAsync("speak", engine, loop, eventLock);
        }

        public async Task StartDtmfAsync(bool eventLock) {
            await ExecuteAsync("start_dtmf", eventLock);
        }

        public async Task StopAsync(bool eventLock = false) {
            await ExecuteAsync("stop_dtmf", eventLock);
        }

        public async Task UnsetAsync(string variableName,
            bool eventLock) {
            await ExecuteAsync("unset", variableName, eventLock);
        }

        public async Task UnsetAsync(string variableName) {
            await ExecuteAsync("unset", variableName, false);
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