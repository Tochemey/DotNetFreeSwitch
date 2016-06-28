using System;
using System.Collections.Generic;
using System.Net;
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
    public abstract class InboundSession : IInboundListener {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     This property holds any additional data that it is required by the InboundSession to run smoothly
        /// </summary>
        public Dictionary<object, object> AdditonalData = new Dictionary<object, object>();

        protected IChannel Channel;
        protected ConnectedCall ConnectedCall;

        public async Task OnConnected(ConnectedCall connectedInfo,
            IChannel channel) {
            ConnectedCall = connectedInfo;
            Channel = channel;
            await ResumeAsync();
            await MyEventsAsync();
            await DivertEventsAsync(true);
            await PreHandleAsync();
            await AnswerAsync();
            await HandleAsync();
        }

        public Task OnDisconnectNotice(EslMessage eslMessage,
            EndPoint channelEndPoint) {
            _logger.Debug("received disconnection message : {0}", eslMessage);
            _logger.Warn("channel {0} disconnected", channelEndPoint);
            return Task.CompletedTask;
        }

        public async Task OnError(Exception exception) {
            if (exception is DecoderException) {
                _logger.Warn(
                    $"Encountered an issue during encoding: {exception}. shutting down...");
                await Channel.CloseAsync();
                return;
            }

            if (exception is SocketException) {
                _logger.Warn(
                    $"Encountered an issue on the channel: {exception}. shutting down...");
                await Channel.CloseAsync();
                return;
            }

            _logger.Error($"Encountered an issue : {exception}");
        }

        public async Task OnEventReceived(EslMessage eslMessage) {
            var eslEvent = new EslEvent(eslMessage);
            var eventName = eslEvent.EventName;
            var eventType = Enumm.Parse<EslEventType>(eventName);
            switch (eventType) {
                case EslEventType.BACKGROUND_JOB:
                    var backgroundJob = new BackgroundJob(eslMessage);
                    eslEvent = backgroundJob;
                    break;
                case EslEventType.CALL_UPDATE:
                    var callUpdate = new CallUpdate(eslMessage);
                    eslEvent = callUpdate;
                    break;
                case EslEventType.CHANNEL_BRIDGE:
                    var channelBridge = new ChannelBridge(eslMessage);
                    eslEvent = channelBridge;
                    break;
                case EslEventType.CHANNEL_HANGUP:
                case EslEventType.CHANNEL_HANGUP_COMPLETE:
                    var channelHangup = new ChannelHangup(eslMessage);
                    eslEvent = channelHangup;
                    break;
                case EslEventType.CHANNEL_PROGRESS:
                    var channelProgress = new ChannelProgress(eslMessage);
                    eslEvent = channelProgress;
                    break;
                case EslEventType.CHANNEL_PROGRESS_MEDIA:
                    var channelProgressMedia = new ChannelProgressMedia(eslMessage);
                    eslEvent = channelProgressMedia;
                    break;
                case EslEventType.CHANNEL_EXECUTE:
                    var channelExecute = new ChannelExecute(eslMessage);
                    eslEvent = channelExecute;
                    break;
                case EslEventType.CHANNEL_EXECUTE_COMPLETE:
                    var channelExecuteComplete = new ChannelExecuteComplete(eslMessage);
                    eslEvent = channelExecuteComplete;
                    break;
                case EslEventType.CHANNEL_UNBRIDGE:
                    var channelUnbridge = new ChannelUnbridge(eslMessage);
                    eslEvent = channelUnbridge;
                    break;
                case EslEventType.SESSION_HEARTBEAT:
                    var sessionHeartbeat = new SessionHeartbeat(eslMessage);
                    eslEvent = sessionHeartbeat;
                    break;
                case EslEventType.DTMF:
                    var dtmf = new Dtmf(eslMessage);
                    eslEvent = dtmf;
                    break;
                case EslEventType.RECORD_STOP:
                    var recordStop = new RecordStop(eslMessage);
                    eslEvent = recordStop;
                    break;
                case EslEventType.CUSTOM:
                    var custom = new Custom(eslMessage);
                    eslEvent = custom;
                    break;
                case EslEventType.CHANNEL_STATE:
                    var channelState = new ChannelStateEvent(eslMessage);
                    eslEvent = channelState;
                    break;
                case EslEventType.CHANNEL_ANSWER:
                    eslEvent = new EslEvent(eslMessage);
                    break;
                case EslEventType.CHANNEL_ORIGINATE:
                    eslEvent = new EslEvent(eslMessage);
                    break;
                case EslEventType.CHANNEL_PARK:
                    var channelPark = new ChannelPark(eslMessage);
                    eslEvent = channelPark;
                    break;
                case EslEventType.CHANNEL_UNPARK:
                    eslEvent = new EslEvent(eslMessage);
                    break;
                default:
                    await OnUnhandledEvents(new EslEvent(eslMessage));
                    break;
            }
            await HandleEvents(eslEvent, eventType);
        }

        public async Task OnRudeRejection() {
            if (Channel != null) await Channel.CloseAsync();
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
            return (@event.UniqueId == ConnectedCall.UniqueId) &&
                   @event.CallerGuid == ConnectedCall.CallerGuid;
        }

        public bool CanSend() {
            return Channel != null && Channel.Active;
        }

        public async Task DivertEventsAsync(bool flag) {
            var command = new DivertEventsCommand(flag);
            await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            string arguments,
            bool eventLock) {
            var command = new SendMsgCommand(ConnectedCall.CallerGuid,
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
            var command = new SendMsgCommand(ConnectedCall.CallerGuid,
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

        public abstract Task HandleAsync();

        public abstract Task HandleEvents(EslEvent @event,
            EslEventType eventType);

        public async Task LingerAsync() {
            await SendCommandAsync(new LingerCommand());
        }

        public async Task MyEventsAsync() {
            await SendCommandAsync(new MyEventsCommand(ConnectedCall.CallerGuid));
        }

        public virtual Task OnUnhandledEvents(EslEvent eslEvent) {
            _logger.Debug("received unhandled freeSwitch event {0}", eslEvent);
            return Task.CompletedTask;
        }

        public async Task PlayAsync(string audioFile,
            bool eventLock = false) {
            await ExecuteAsync("playback", audioFile, eventLock);
        }

        public async Task PreAnswerAsync() {
            await ExecuteAsync("pre_answer");
        }

        public abstract Task PreHandleAsync();

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
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand, Channel);
            return response;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand) {
            if (!CanSend()) return Guid.Empty;
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand, Channel);
        }

        public async Task<CommandReply> SendCommandAsync(BaseCommand command) {
            if (!CanSend()) return null;
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command, Channel);
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

        public async Task StopDtmfAsync(bool eventLock = false) {
            await ExecuteAsync("stop_dtmf", eventLock);
        }

        public async Task UnsetAsync(string variableName,
            bool eventLock) {
            await ExecuteAsync("unset", variableName, eventLock);
        }

        public async Task UnsetAsync(string variableName) {
            await ExecuteAsync("unset", variableName, false);
        }
    }
}