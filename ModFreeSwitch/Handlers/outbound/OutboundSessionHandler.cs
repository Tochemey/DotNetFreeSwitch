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
using DotNetty.Transport.Channels;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.outbound
{
    /// <summary>
    ///     OutboundSessionHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class OutboundSessionHandler : EslSessionHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IOutboundListener _outboundListener;


        public OutboundSessionHandler(IOutboundListener outboundListener)
        {
            _outboundListener = outboundListener;
        }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception)
        {
            _logger.Error(exception, "Exception occured.");
            await _outboundListener.OnError(exception);
        }

        public override async void ChannelRead(IChannelHandlerContext context,
            object message)
        {
            var eslMessage = message as EslMessage;
            var contentType = eslMessage?.ContentType();

            if (string.IsNullOrEmpty(contentType)) return;

            // Handle auth/request
            if (contentType.Equals(EslHeadersValues.AuthRequest))
            {
                await _outboundListener.OnAuthentication();
                return;
            }

            // Handle command/reply
            if (contentType.Equals(EslHeadersValues.CommandReply))
            {
                var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                var reply = new CommandReply(commandAsyncEvent.Command.Command, eslMessage);
                commandAsyncEvent.Complete(reply);
                return;
            }

            // Handle api/response
            if (contentType.Equals(EslHeadersValues.ApiResponse))
            {
                var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                var apiResponse = new ApiResponse(commandAsyncEvent.Command.Command,
                    eslMessage);
                commandAsyncEvent.Complete(apiResponse);
                return;
            }

            // Handle text/event-plain
            if (contentType.Equals(EslHeadersValues.TextEventPlain))
            {
                await _outboundListener.OnEventReceived(eslMessage);
                return;
            }

            // Handle disconnect/notice message
            if (contentType.Equals(EslHeadersValues.TextDisconnectNotice))
            {
                var channel = context.Channel;
                var address = channel.RemoteAddress;

                await _outboundListener.OnDisconnectNotice(eslMessage, address);
                return;
            }

            // Handle rude/rejection message
            if (contentType.Equals(EslHeadersValues.TextRudeRejection))
            {
                await _outboundListener.OnRudeRejection();
                return;
            }

            // Unexpected freeSwitch message
            _logger.Warn("Unexpected message content type [{0}]", contentType);
        }
    }
}