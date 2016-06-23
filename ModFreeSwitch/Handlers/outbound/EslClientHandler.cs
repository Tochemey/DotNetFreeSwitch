using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.outbound {
    /// <summary>
    ///     EslClientHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class EslClientHandler : ChannelHandlerAdapter {
        /// <summary>
        ///     Helps process api command sequentially however in a asynchronous manner
        /// </summary>
        private readonly Queue<CommandAsyncEvent> _commandAsyncEvents;

        private readonly IEventListener _eslEventListener;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     This password is used to connect to mod_event_socket module of freeSwitch.
        /// </summary>
        private readonly string _password;

        public EslClientHandler(string password,
            IEventListener eslEventListener) {
            _password = password;
            _eslEventListener = eslEventListener;
            _commandAsyncEvents = new Queue<CommandAsyncEvent>();
        }

        public EslClientHandler(IEventListener eslEventListener)
            : this("ClueCon", eslEventListener) {}

        public bool Authenticated { get; private set; }

        /// <summary>
        ///     This helps read the data received from the socket client.
        /// </summary>
        /// <param name="context">Channel context</param>
        /// <param name="message">the decoded message received</param>
        public override async void ChannelRead(IChannelHandlerContext context,
            object message) {
            var eslMessage = message as EslMessage;
            if (eslMessage == null) return;
            var contentType = eslMessage.ContentType();
            if (contentType.Equals(EslHeadersValues.AuthRequest)) {
                await Authenticate(context.Channel);
                return;
            }

            // Handle command/reply
            if (contentType.Equals(EslHeadersValues.CommandReply)) {
                var commandAsyncEvent = _commandAsyncEvents.Dequeue();
                var reply = new CommandReply(commandAsyncEvent.Command.Command, eslMessage);
                commandAsyncEvent.Complete(reply);
                return;
            }

            // Handle api/response
            if (contentType.Equals(EslHeadersValues.ApiResponse)) {
                var commandAsyncEvent = _commandAsyncEvents.Dequeue();
                var apiResponse = new ApiResponse(
                    commandAsyncEvent.Command.Command,
                    eslMessage);
                commandAsyncEvent.Complete(apiResponse);
                return;
            }

            // Handle text/event-plain
            if (contentType.Equals(EslHeadersValues.TextEventPlain)) {
                EslEvent eslEvent;
                if (eslMessage.HasHeader("Event-Name") &&
                    eslMessage.Headers["Event-Name"].Equals("CHANNEL_DATA")) {
                    eslEvent = new EslEvent(eslMessage, true);
                }
                else {
                    eslEvent = new EslEvent(eslMessage);
                }
                await _eslEventListener.OnEventReceived(eslEvent);
                return;
            }

            // Unexpected freeSwitch message
            _logger.Warn("Unexpected message content type [{0}]", contentType);
        }

        public async Task<ApiResponse> SendApi(ApiCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as ApiResponse;
        }

        public async Task<Guid> SendBgApi(BgApiCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            var jobUuid = Guid.Empty;
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            var reply = await asyncEvent.Task as CommandReply;
            if (reply == null) return jobUuid;
            if (reply.IsOk)
                return Guid.TryParse(reply[EslHeaders.JobUuid], out jobUuid)
                    ? jobUuid
                    : Guid.Empty;
            return jobUuid;
        }

        public async Task<CommandReply> SendCommand(BaseCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as CommandReply;
        }

        protected async Task Authenticate(IChannel context) {
            var command = new AuthCommand(_password);
            var reply = await SendCommand(command, context);
            Authenticated = reply.IsOk;
        }
    }
}