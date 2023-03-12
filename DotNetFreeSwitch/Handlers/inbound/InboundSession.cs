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
using DotNetFreeSwitch.Commands;
using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Events;
using DotNetFreeSwitch.Messages;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace DotNetFreeSwitch.Handlers.inbound
{
    /// <summary>
    /// InboundSession is used to implement a freeswitch Outbound Event Socket application server.
    /// </summary>
    public abstract class InboundSession : IInboundListener
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected IChannel Channel;
        protected InboundCall InboundCall;

        /// <summary>
        ///     This property holds any additional data that it is required by the InboundSession to run smoothly
        /// </summary>
        public Dictionary<object, object> Meta = new Dictionary<object, object>();

        /// <summary>
        /// OnConnected is called whenever freeswitch connects to the InboundServer
        /// </summary>
        /// <param name="connectedInfo">the connection info</param>
        /// <param name="channel">the tcp socket channel</param>
        /// <returns></returns>
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

        /// <summary>
        /// OnDisconnectNotice is called whenever a freeswitch connection to the InboundServer
        /// is disconnected
        /// </summary>
        /// <param name="message">the disconnection message</param>
        /// <param name="channelEndPoint">the tcp channel endpoint</param>
        /// <returns></returns>
        public Task OnDisconnectNotice(Message message,
            EndPoint channelEndPoint)
        {
            _logger.Debug("received disconnection message : {0}",
                message);
            _logger.Warn("channel {0} disconnected",
                channelEndPoint);
            return Task.CompletedTask;
        }

        /// <summary>
        /// OnError is executed whenever an error occur
        /// </summary>
        /// <param name="exception">the error</param>
        /// <returns></returns>
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

        /// <summary>
        /// OnEventReceived is executed whenever an freeswitch event is emitted on the wire
        /// </summary>
        /// <param name="message">the event message</param>
        public void OnEventReceived(Message message)
        {
            // wrap the message into a freeswitch event
            var eslEvent = new FsEvent(message);
            // grab the event name
            var eventName = eslEvent.EventName;
            // grab the event event type
            var eventType = EnumExtensions.Parse<EventType>(eventName);
            // pattern-match on the event type to appropriately dispatch the right
            // the event handler
            switch (eventType)
            {
                case EventType.BACKGROUND_JOB:
                    var backgroundJob = new BackgroundJob(message);
                    eslEvent = backgroundJob;
                    break;
                case EventType.CALL_UPDATE:
                    var callUpdate = new CallUpdate(message);
                    eslEvent = callUpdate;
                    break;
                case EventType.CHANNEL_BRIDGE:
                    var channelBridge = new ChannelBridge(message);
                    eslEvent = channelBridge;
                    break;
                case EventType.CHANNEL_HANGUP:
                case EventType.CHANNEL_HANGUP_COMPLETE:
                    var channelHangup = new ChannelHangup(message);
                    eslEvent = channelHangup;
                    break;
                case EventType.CHANNEL_PROGRESS:
                    var channelProgress = new ChannelProgress(message);
                    eslEvent = channelProgress;
                    break;
                case EventType.CHANNEL_PROGRESS_MEDIA:
                    var channelProgressMedia = new ChannelProgressMedia(message);
                    eslEvent = channelProgressMedia;
                    break;
                case EventType.CHANNEL_EXECUTE:
                    var channelExecute = new ChannelExecute(message);
                    eslEvent = channelExecute;
                    break;
                case EventType.CHANNEL_EXECUTE_COMPLETE:
                    var channelExecuteComplete = new ChannelExecuteComplete(message);
                    eslEvent = channelExecuteComplete;
                    break;
                case EventType.CHANNEL_UNBRIDGE:
                    var channelUnbridge = new ChannelUnbridge(message);
                    eslEvent = channelUnbridge;
                    break;
                case EventType.SESSION_HEARTBEAT:
                    var sessionHeartbeat = new SessionHeartbeat(message);
                    eslEvent = sessionHeartbeat;
                    break;
                case EventType.DTMF:
                    var dtmf = new Dtmf(message);
                    eslEvent = dtmf;
                    break;
                case EventType.RECORD_STOP:
                    var recordStop = new RecordStop(message);
                    eslEvent = recordStop;
                    break;
                case EventType.CUSTOM:
                    var custom = new Custom(message);
                    eslEvent = custom;
                    break;
                case EventType.CHANNEL_STATE:
                    var channelState = new ChannelStateEvent(message);
                    eslEvent = channelState;
                    break;
                case EventType.CHANNEL_ANSWER:
                    eslEvent = new FsEvent(message);
                    break;
                case EventType.CHANNEL_ORIGINATE:
                    eslEvent = new FsEvent(message);
                    break;
                case EventType.CHANNEL_PARK:
                    var channelPark = new ChannelPark(message);
                    eslEvent = channelPark;
                    break;
                case EventType.CHANNEL_UNPARK:
                    eslEvent = new FsEvent(message);
                    break;
                default:
                    OnUnhandledEvents(new FsEvent(message));
                    break;
            }

            // handle the given event that has been emitted
            HandleEvents(eslEvent,
                eventType);
        }

        /// <summary>
        /// OnRudeRejection is emitted when a freeswitch session is closed
        /// </summary>
        /// <returns></returns>
        public async Task OnRudeRejection()
        {
            if (Channel != null) await Channel.CloseAsync();
        }

        /// <summary>
        /// AnswerAsync is used to Answer the call for the connected channel.
        /// </summary>
        /// <returns></returns>
        public async Task AnswerAsync()
        {
            await ExecuteAsync("answer",
                true);
        }

        /// <summary>
        /// BindDigitActionAsync is used to Bind a key sequence or regex to an action.
        /// </summary>
        /// <param name="command">the freeswitch api command to be executed</param>
        /// <param name="eventLock">states whether to force synchronous operations in async mode</param>
        /// <returns></returns>
        public async Task BindDigitActionAsync(string command,
            bool eventLock = true)
        {
            await ExecuteAsync("bind_digit_action",
                command,
                eventLock);
        }

        /// <summary>
        /// BridgeAsync is used to Bridge a new channel to the existing one.
        /// </summary>
        /// <param name="bridgeText">the freeswitch bridge text</param>
        /// <param name="eventLock">states whether to force synchronous operations in async mode</param>
        /// <returns></returns>
        public async Task BridgeAsync(string bridgeText,
            bool eventLock = false)
        {
            await ExecuteAsync("bridge",
                bridgeText,
                eventLock);
        }

        /// <summary>
        /// CanHandleEvent is used to check whether the connected freeswitch channel can process a given event.
        /// This means that the connected channel should subscribe to the given freeswitch event prior to handling it.
        /// </summary>
        /// <param name="event">the given freeswitch event to handle</param>
        /// <returns>true when the event can be handled and false on the contrary</returns>
        public bool CanHandleEvent(FsEvent @event)
        {
            return @event.UniqueId == InboundCall.UniqueId && @event.CallerGuid == InboundCall.CallerGuid;
        }

        /// <summary>
        /// IsChannelReady is used to check whether the connected freeswitch channel is ready
        /// </summary>
        /// <returns>true when the channel is ready and false on the contrary</returns>
        public bool IsChannelReady()
        {
            return Channel != null && Channel.Active;
        }

        /// <summary>
        /// Sends a divert events command asynchronously.
        /// This allow events that an embedded script would expect to get in the 
        /// inputcallback to be diverted to the event socket. When the flag is true then events are diverted
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task DivertEventsAsync(bool flag)
        {
            var command = new DivertEventsCommand(flag);
            await SendCommandAsync(command);
        }

        /// <summary>
        /// Executes application commands asynchronously and returns a command reply
        /// </summary>
        /// <param name="application">the application to execute</param>
        /// <param name="arguments">the application arguments</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns>the command reply</returns>
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

        /// <summary>
        /// Executes application commands asynchronously and returns a command reply
        /// </summary>
        /// <param name="application">the application to execute</param>
        /// <param name="arguments">the application arguments</param>
        /// <param name="loop">the number of times to execute the application command</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>/
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

        /// <summary>
        /// Executes application commands asynchronously and returns a command reply with default settings
        /// </summary>
        /// <param name="application">the application to execute</param>
        /// <returns></returns>
        public async Task<CommandReply> ExecuteAsync(string application)
        {
            return await ExecuteAsync(application,
                string.Empty,
                false);
        }

        /// <summary>
        /// Executes application commands asynchronously and returns a command reply.
        /// </summary>
        /// <param name="application">the application to execute</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
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

        /// <summary>
        /// Executes the linger command. 
        /// It tells FreeSWITCH not to close the socket connection when a channel hangs up. 
        /// Instead, it keeps the socket connection open until the last event related to 
        /// the channel has been received by the socket client.
        /// </summary>
        /// <returns></returns>
        public async Task LingerAsync()
        {
            await SendCommandAsync(new LingerCommand());
        }

        /// <summary>
        /// Executes my events command
        /// </summary>
        /// <returns></returns>
        public async Task MyEventsAsync()
        {
            await SendCommandAsync(new MyEventsCommand(InboundCall.CallerGuid));
        }

        /// <summary>
        /// OnUnhandledEvents is executed when unhandled events are received
        /// </summary>
        /// <param name="fsEvent">the unhandled event</param>
        /// <returns></returns>
        protected virtual Task OnUnhandledEvents(FsEvent fsEvent)
        {
            _logger.Debug("received unhandled freeSwitch event {0}",
                fsEvent);
            return Task.CompletedTask;
        }

        /// <summary>
        ///  Sends a playback command
        /// </summary>
        /// <param name="audioFile">the audio file</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>/
        protected async Task PlayAsync(string audioFile,
            bool eventLock = false)
        {
            await ExecuteAsync("playback",
                audioFile,
                eventLock);
        }

        /// <summary>
        /// Sends a pre-answer command
        /// </summary>
        /// <returns></returns>
        public async Task PreAnswerAsync()
        {
            await ExecuteAsync("pre_answer");
        }

        protected abstract Task PreHandleAsync();

        /// <summary>
        /// Send resume command
        /// </summary>
        /// <returns></returns>
        public async Task ResumeAsync()
        {
            await SendCommandAsync(new ResumeCommand());
        }

        /// <summary>
        /// Executes an ring_ready application
        /// </summary>
        /// <returns></returns>
        public async Task RingReadyAsync()
        {
            await ExecuteAsync("ring_ready",
                true);
        }

        /// <summary>
        /// Executes an ring_ready application with an event lock
        /// </summary>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task RingReadyAsync(bool eventLock)
        {
            await ExecuteAsync("ring_ready",
                eventLock);
        }

        /// <summary>
        /// Executes the say application. 
        /// The say application will use the pre-recorded sound files to read or say various things like dates, times, digits, etc.
        /// </summary>
        /// <param name="language">the language</param>
        /// <param name="type">the say type</param>
        /// <param name="method">the say method</param>
        /// <param name="gender">the gender</param>
        /// <param name="text">the text</param>
        /// <param name="loop">how many times to run the say application</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends an api command and returns an api response
        /// </summary>
        /// <param name="apiCommand">the api command</param>
        /// <returns>the api response</returns>
        public async Task<ApiResponse> SendApiAsync(ApiCommand apiCommand)
        {
            if (!IsChannelReady()) return null;
            var handler = (InboundSessionHandler)Channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand,
                Channel);
            return response;
        }

        /// <summary>
        /// Sends a background api command and returns the UUID attached to it
        /// </summary>
        /// <param name="bgApiCommand">the background api command</param>
        /// <returns>the UUID of the command sent</returns>
        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand)
        {
            if (!IsChannelReady()) return Guid.Empty;
            var handler = (InboundSessionHandler)Channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand,
                Channel);
        }

        /// <summary>
        /// Sends a command to freeswitch and returns a command reply
        /// </summary>
        /// <param name="command">the command</param>
        /// <returns>the command reply</returns>
        public async Task<CommandReply> SendCommandAsync(BaseCommand command)
        {
            if (!IsChannelReady()) return null;
            var handler = (InboundSessionHandler)Channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command,
                Channel);
            return reply;
        }

        /// <summary>
        /// Sets a variable
        /// </summary>
        /// <param name="variableName">the variable name</param>
        /// <param name="variableValue">the variable value</param>
        /// <returns></returns>
        public async Task SetAsync(string variableName,
            string variableValue)
        {
            await ExecuteAsync("set",
                variableName + "=" + variableValue,
                false);
        }

        /// <summary>
        ///  Sets a channel variable for the channel calling the application.
        /// </summary>
        /// <param name="variableName">the variable name</param>
        /// <param name="variableValue">the variable value</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task SetAsync(string variableName,
            string variableValue,
            bool eventLock)
        {
            await ExecuteAsync("set",
                variableName + "=" + variableValue,
                eventLock);
        }

        /// <summary>
        /// Sleeps. its pauses the channel for a given number of milliseconds, consuming the audio for that period of time.
        /// </summary>
        /// <param name="millisecond">the given number of milliseconds</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task SleepAsync(int millisecond,
            bool eventLock = false)
        {
            await ExecuteAsync("sleep",
                Convert.ToString(millisecond),
                eventLock);
        }

        /// <summary>
        /// Executes the speak application.
        /// It speaks a string or file of text to the channel using the defined text-to-speech engine.
        /// </summary>
        /// <param name="engine">the text-to-speech engine</param>
        /// <param name="voice">the voice</param>
        /// <param name="text">the text to speak</param>
        /// <param name="timerName">the timer name</param>
        /// <param name="loop">how many times to speak the text</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
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

        /// <summary>
        ///  Executes the speak application.
        ///  It speaks a string or file of text to the channel using the defined text-to-speech engine.
        /// </summary>
        /// <param name="engine">the text-to-speech engine</param>
        /// <param name="loop">how many times to speak the text</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task SpeakAsync(string engine,
            int loop = 1,
            bool eventLock = false)
        {
            await ExecuteAsync("speak",
                engine,
                loop,
                eventLock);
        }

        /// <summary>
        /// Execute the start_dtmf application.
        /// One can can use start_dtmf in a dialplan to enable in-band DTMF detection (i.e. the detection of DTMF tones on a channel). 
        /// You should do this when you want to be able to identify DTMF tones on a channel 
        /// that doesn't otherwise support another signaling method (like RFC2833 or INFO).
        /// </summary>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task StartDtmfAsync(bool eventLock)
        {
            await ExecuteAsync("start_dtmf",
                eventLock);
        }

        [Obsolete("kindly use the StopDtmfAsync instead")]
        /// <summary>
        /// Execute the stop_dtmf application. It stops in-band DTMF detection.
        /// </summary>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task StopAsync(bool eventLock = false)
        {
            await ExecuteAsync("stop_dtmf",
                eventLock);
        }

        /// <summary>
        /// Execute the stop_dtmf application. It stops in-band DTMF detection.
        /// </summary>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task StopDtmfAsync(bool eventLock = false)
        {
            await ExecuteAsync("stop_dtmf",
                eventLock);
        }

        /// <summary>
        /// Executes the unset application. This unset a channel variables
        /// </summary>
        /// <param name="variableName">the channel variable name</param>
        /// <param name="eventLock">states whether to force synchronous execution in async mode on freeswitch</param>
        /// <returns></returns>
        public async Task UnsetAsync(string variableName,
            bool eventLock)
        {
            await ExecuteAsync("unset",
                variableName,
                eventLock);
        }

        /// <summary>
        /// Executes the unset application. This unset a channel variables
        /// </summary>
        /// <param name="variableName">the channel variable name</param>
        /// <returns></returns>
        public async Task UnsetAsync(string variableName)
        {
            await ExecuteAsync("unset",
                variableName,
                false);
        }
    }
}
