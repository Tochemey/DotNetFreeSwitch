using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundSessionHandler : ChannelHandlerAdapter {
        /// <summary>
        ///     Helps process api command sequentially however in a asynchronous manner
        /// </summary>
        private readonly Queue<CommandAsyncEvent> _commandAsyncEvents;

        private readonly IInboundListener _inboundListener;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InboundSessionHandler(IInboundListener inboundListener) {
            _inboundListener = inboundListener;
            _commandAsyncEvents = new Queue<CommandAsyncEvent>();
        }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception) {
            _logger.Error(exception, "Exception occured.");
            await _inboundListener.OnError(exception);
        }

        public override async void ChannelActive(IChannelHandlerContext context) {
            var channel = context.Channel;
            _logger.Debug(
                "received a new connection from freeswitch {0}. Sending a connect command...",
                channel.LocalAddress);
            var connectCommand = new ConnectCommand();
            var reply = await SendCommandAsync(connectCommand, channel);
            if (!reply.IsOk) return;
            var connectedCall = new ConnectedCall(new EslEvent(reply.Response, true));
            await _inboundListener.OnConnected(connectedCall);
        }

        public override async void ChannelRead(IChannelHandlerContext context,
            object message) {
            var eslMessage = message as EslMessage;
            if (eslMessage == null) return;
            var contentType = eslMessage.ContentType();

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

                await _inboundListener.OnEventReceived(eslEvent);
                return;
            }

            // Handle disconnect/notice message
            if (contentType.Equals(EslHeadersValues.TextDisconnectNotice)) {
                await _inboundListener.OnDisconnectNotice();
                return;
            }

            // Handle rude/rejection message
            if (contentType.Equals(EslHeadersValues.TextRudeRejection)) {
                await _inboundListener.OnRudeRejection();
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
    }
}