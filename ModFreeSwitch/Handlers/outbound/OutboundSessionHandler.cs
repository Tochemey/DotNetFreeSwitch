using System;
using System.Net;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.outbound {
    /// <summary>
    ///     OutboundSessionHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class OutboundSessionHandler : EslSessionHandler {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IOutboundListener _outboundListener;


        public OutboundSessionHandler(IOutboundListener outboundListener) {
            _outboundListener = outboundListener;
        }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception) {
            _logger.Error(exception, "Exception occured.");
            await _outboundListener.OnError(exception);
        }

        public override async void ChannelRead(IChannelHandlerContext context,
            object message) {
            var eslMessage = message as EslMessage;
            var contentType = eslMessage?.ContentType();

            if (string.IsNullOrEmpty(contentType)) return;

            // Handle auth/request
            if (contentType.Equals(EslHeadersValues.AuthRequest)) {
                await _outboundListener.OnAuthentication();
                return;
            }

            // Handle command/reply
            if (contentType.Equals(EslHeadersValues.CommandReply)) {
                var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                var reply = new CommandReply(commandAsyncEvent.Command.Command, eslMessage);
                commandAsyncEvent.Complete(reply);
                return;
            }

            // Handle api/response
            if (contentType.Equals(EslHeadersValues.ApiResponse)) {
                var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                var apiResponse = new ApiResponse(commandAsyncEvent.Command.Command,
                    eslMessage);
                commandAsyncEvent.Complete(apiResponse);
                return;
            }

            // Handle text/event-plain
            if (contentType.Equals(EslHeadersValues.TextEventPlain)) {
                await _outboundListener.OnEventReceived(eslMessage);
                return;
            }

            // Handle disconnect/notice message
            if (contentType.Equals(EslHeadersValues.TextDisconnectNotice)) {
                IChannel channel = context.Channel;
                EndPoint address = channel.RemoteAddress;

                await _outboundListener.OnDisconnectNotice(eslMessage, address);
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
    }
}