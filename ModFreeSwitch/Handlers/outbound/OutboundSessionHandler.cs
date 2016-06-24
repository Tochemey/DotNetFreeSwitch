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
    ///     OutboundSessionHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class OutboundSessionHandler : ChannelHandlerAdapter {
        /// <summary>
        ///     Helps process api command sequentially however in a asynchronous manner
        /// </summary>
        private readonly Queue<CommandAsyncEvent> _commandAsyncEvents;

        private readonly IOutboundListener _outboundListener;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     This password is used to connect to mod_event_socket module of freeSwitch.
        /// </summary>
        private readonly string _password;

        public OutboundSessionHandler(string password,
            IOutboundListener outboundListener) {
            _password = password;
            _outboundListener = outboundListener;
            _commandAsyncEvents = new Queue<CommandAsyncEvent>();
        }

        public OutboundSessionHandler(IOutboundListener outboundListener)
            : this("ClueCon", outboundListener) {}

        public bool Authenticated { get; private set; }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception) {
            _logger.Error(exception, "Exception occured.");
            await _outboundListener.OnError(exception);
        }

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
                await AuthenticateAsync(context.Channel);
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
                var apiResponse = new ApiResponse(commandAsyncEvent.Command.Command,
                    eslMessage);
                commandAsyncEvent.Complete(apiResponse);
                return;
            }

            // Handle text/event-plain
            if (contentType.Equals(EslHeadersValues.TextEventPlain)) {
                var eslEvent = new EslEvent(eslMessage);

                await _outboundListener.OnEventReceived(eslEvent);
                return;
            }

            // Handle disconnect/notice message
            if (contentType.Equals(EslHeadersValues.TextDisconnectNotice)) {
                await _outboundListener.OnDisconnectNotice();
                return;
            }

            // Handle rude/rejection message
            if (contentType.Equals(EslHeadersValues.TextRudeRejection)) {
                await _outboundListener.OnRudeRejection();
                return;
            }

            // Unexpected freeSwitch message
            _logger.Warn("Unexpected message content type [{0}]", contentType);
        }

        public async Task<ApiResponse> SendApiAsync(ApiCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as ApiResponse;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand command,
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

        public async Task<CommandReply> SendCommandAsync(BaseCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as CommandReply;
        }

        protected async Task AuthenticateAsync(IChannel context) {
            var command = new AuthCommand(_password);
            var reply = await SendCommandAsync(command, context);
            Authenticated = reply.IsOk;
        }
    }
}