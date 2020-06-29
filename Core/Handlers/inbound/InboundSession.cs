/*
    Copyright [2016] [Arsene Tochemey GANDOTE]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Commands;
using Core.Common;
using Core.Events;
using Core.Messages;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace Core.Handlers.inbound
{
    public abstract class InboundSession : IInboundListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected IChannel Channel;
        protected InboundCall InboundCall;

        /// <summary>
        ///     This property holds any additional data that it is required by the InboundSession to run smoothly
        /// </summary>
        public Dictionary<object, object> Meta = new Dictionary<object, object>();

        public async Task OnConnected(InboundCall connectedInfo,
            IChannel channel)
        {
            InboundCall = connectedInfo;
            Channel = channel;
            await ResumeAsync();
            await MyEventsAsync();
            await DivertEventsAsync(true);
            await PreHandleAsync();
            await AnswerAsync();
            await HandleAsync();
        }

        public Task OnDisconnectNotice(FsMessage fsMessage,
            EndPoint channelEndPoint)
        {
            _logger.Debug("received disconnection message : {0}",
                fsMessage);
            _logger.Warn("channel {0} disconnected",
                channelEndPoint);
            return Task.CompletedTask;
        }

        public async Task OnError(Exception exception)
        {
            switch (exception)
            {
                case DecoderException _:
                    _logger.Warn($"Encountered an issue during encoding: {exception}. shutting down...");
                    await Channel.CloseAsync();
                    return;
                case SocketException _:
                    _logger.Warn($"Encountered an issue on the channel: {exception}. shutting down...");
                    await Channel.CloseAsync();
                    return;
                default:
                    _logger.Error($"Encountered an issue : {exception}");
                    break;
            }
        }

        public void OnEventReceived(FsMessage fsMessage)
        {
            var eslEvent = new FsEvent(fsMessage);
            var eventName = eslEvent.EventName;
            var eventType = EnumExtensions.Parse<EventType>(eventName);
            switch (eventType)
            {
                case EventType.BACKGROUND_JOB:
                    var backgroundJob = new BackgroundJob(fsMessage);
                    eslEvent = backgroundJob;
                    break;
                case EventType.CALL_UPDATE:
                    var callUpdate = new CallUpdate(fsMessage);
                    eslEvent = callUpdate;
                    break;
                case EventType.CHANNEL_BRIDGE:
                    var channelBridge = new ChannelBridge(fsMessage);
                    eslEvent = channelBridge;
                    break;
                case EventType.CHANNEL_HANGUP:
                case EventType.CHANNEL_HANGUP_COMPLETE:
                    var channelHangup = new ChannelHangup(fsMessage);
                    eslEvent = channelHangup;
                    break;
                case EventType.CHANNEL_PROGRESS:
                    var channelProgress = new ChannelProgress(fsMessage);
                    eslEvent = channelProgress;
                    break;
                case EventType.CHANNEL_PROGRESS_MEDIA:
                    var channelProgressMedia = new ChannelProgressMedia(fsMessage);
                    eslEvent = channelProgressMedia;
                    break;
                case EventType.CHANNEL_EXECUTE:
                    var channelExecute = new ChannelExecute(fsMessage);
                    eslEvent = channelExecute;
                    break;
                case EventType.CHANNEL_EXECUTE_COMPLETE:
                    var channelExecuteComplete = new ChannelExecuteComplete(fsMessage);
                    eslEvent = channelExecuteComplete;
                    break;
                case EventType.CHANNEL_UNBRIDGE:
                    var channelUnbridge = new ChannelUnbridge(fsMessage);
                    eslEvent = channelUnbridge;
                    break;
                case EventType.SESSION_HEARTBEAT:
                    var sessionHeartbeat = new SessionHeartbeat(fsMessage);
                    eslEvent = sessionHeartbeat;
                    break;
                case EventType.DTMF:
                    var dtmf = new Dtmf(fsMessage);
                    eslEvent = dtmf;
                    break;
                case EventType.RECORD_STOP:
                    var recordStop = new RecordStop(fsMessage);
                    eslEvent = recordStop;
                    break;
                case EventType.CUSTOM:
                    var custom = new Custom(fsMessage);
                    eslEvent = custom;
                    break;
                case EventType.CHANNEL_STATE:
                    var channelState = new ChannelStateEvent(fsMessage);
                    eslEvent = channelState;
                    break;
                case EventType.CHANNEL_ANSWER:
                    eslEvent = new FsEvent(fsMessage);
                    break;
                case EventType.CHANNEL_ORIGINATE:
                    eslEvent = new FsEvent(fsMessage);
                    break;
                case EventType.CHANNEL_PARK:
                    var channelPark = new ChannelPark(fsMessage);
                    eslEvent = channelPark;
                    break;
                case EventType.CHANNEL_UNPARK:
                    eslEvent = new FsEvent(fsMessage);
                    break;
                default:
                    OnUnhandledEvents(new FsEvent(fsMessage));
                    break;
            }

            HandleEvents(eslEvent,
                eventType);
        }

        public async Task OnRudeRejection()
        {
            if (Channel != null) await Channel.CloseAsync();
        }

        public async Task AnswerAsync()
        {
            await ExecuteAsync("answer",
                true);
        }

        public async Task BindDigitActionAsync(string command,
            bool eventLock = true)
        {
            await ExecuteAsync("bind_digit_action",
                command,
                eventLock);
        }

        public async Task BridgeAsync(string bridgeText,
            bool eventLock = false)
        {
            await ExecuteAsync("bridge",
                bridgeText,
                eventLock);
        }

        public bool CanHandleEvent(FsEvent @event)
        {
            return @event.UniqueId == InboundCall.UniqueId && @event.CallerGuid == InboundCall.CallerGuid;
        }

        public bool IsChannelReady()
        {
            return Channel != null && Channel.Active;
        }

        public async Task DivertEventsAsync(bool flag)
        {
            var command = new DivertEventsCommand(flag);
            await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            string arguments,
            bool eventLock)
        {
            var command = new SendMsgCommand(InboundCall.CallerGuid,
                SendMsgCommand.CallCommand,
                application,
                arguments,
                eventLock);
            return await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            string arguments,
            int loop,
            bool eventLock)
        {
            var command = new SendMsgCommand(InboundCall.CallerGuid,
                SendMsgCommand.CallCommand,
                application,
                arguments,
                eventLock,
                loop);
            return await SendCommandAsync(command);
        }

        public async Task<CommandReply> ExecuteAsync(string application)
        {
            return await ExecuteAsync(application,
                string.Empty,
                false);
        }

        public async Task<CommandReply> ExecuteAsync(string application,
            bool eventLock)
        {
            return await ExecuteAsync(application,
                string.Empty,
                eventLock);
        }

        protected abstract Task HandleAsync();

        protected abstract Task HandleEvents(FsEvent @event,
            EventType eventType);

        public async Task LingerAsync()
        {
            await SendCommandAsync(new LingerCommand());
        }

        public async Task MyEventsAsync()
        {
            await SendCommandAsync(new MyEventsCommand(InboundCall.CallerGuid));
        }

        protected virtual Task OnUnhandledEvents(FsEvent fsEvent)
        {
            _logger.Debug("received unhandled freeSwitch event {0}",
                fsEvent);
            return Task.CompletedTask;
        }

        protected async Task PlayAsync(string audioFile,
            bool eventLock = false)
        {
            await ExecuteAsync("playback",
                audioFile,
                eventLock);
        }

        public async Task PreAnswerAsync()
        {
            await ExecuteAsync("pre_answer");
        }

        protected abstract Task PreHandleAsync();

        public async Task ResumeAsync()
        {
            await SendCommandAsync(new ResumeCommand());
        }

        public async Task RingReadyAsync()
        {
            await ExecuteAsync("ring_ready",
                true);
        }

        public async Task RingReadyAsync(bool eventLock)
        {
            await ExecuteAsync("ring_ready",
                eventLock);
        }

        public async Task SayAsync(string language,
            SayTypes type,
            SayMethods method,
            SayGenders gender,
            string text,
            int loop,
            bool eventLock)
        {
            await ExecuteAsync("say",
                language + " " + type + " " + method.ToString().Replace("_",
                    "/") + " " + gender + " " + text,
                loop,
                eventLock);
        }

        public async Task<ApiResponse> SendApiAsync(ApiCommand apiCommand)
        {
            if (!IsChannelReady()) return null;
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand,
                Channel);
            return response;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand)
        {
            if (!IsChannelReady()) return Guid.Empty;
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand,
                Channel);
        }

        public async Task<CommandReply> SendCommandAsync(BaseCommand command)
        {
            if (!IsChannelReady()) return null;
            var handler = (InboundSessionHandler) Channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command,
                Channel);
            return reply;
        }

        public async Task SetAsync(string variableName,
            string variableValue)
        {
            await ExecuteAsync("set",
                variableName + "=" + variableValue,
                false);
        }

        public async Task SetAsync(string variableName,
            string variableValue,
            bool eventLock)
        {
            await ExecuteAsync("set",
                variableName + "=" + variableValue,
                eventLock);
        }

        public async Task SleepAsync(int millisecond,
            bool eventLock = false)
        {
            await ExecuteAsync("sleep",
                Convert.ToString(millisecond),
                eventLock);
        }

        public async Task SpeakAsync(string engine,
            string voice,
            string text,
            string timerName = null,
            int loop = 1,
            bool eventLock = false)
        {
            await ExecuteAsync("speak",
                engine + "|" + voice + "|" + text + (!string.IsNullOrEmpty(timerName) ? "|" + timerName : ""),
                loop,
                eventLock);
        }

        public async Task SpeakAsync(string engine,
            int loop = 1,
            bool eventLock = false)
        {
            await ExecuteAsync("speak",
                engine,
                loop,
                eventLock);
        }

        public async Task StartDtmfAsync(bool eventLock)
        {
            await ExecuteAsync("start_dtmf",
                eventLock);
        }

        public async Task StopAsync(bool eventLock = false)
        {
            await ExecuteAsync("stop_dtmf",
                eventLock);
        }

        public async Task StopDtmfAsync(bool eventLock = false)
        {
            await ExecuteAsync("stop_dtmf",
                eventLock);
        }

        public async Task UnsetAsync(string variableName,
            bool eventLock)
        {
            await ExecuteAsync("unset",
                variableName,
                eventLock);
        }

        public async Task UnsetAsync(string variableName)
        {
            await ExecuteAsync("unset",
                variableName,
                false);
        }
    }
}