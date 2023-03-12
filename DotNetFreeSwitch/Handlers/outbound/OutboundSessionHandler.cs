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
using DotNetFreeSwitch.Messages;
using DotNetty.Transport.Channels;
using NLog;

namespace DotNetFreeSwitch.Handlers.outbound
{
    /// <summary>
    ///     OutboundSessionHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class OutboundSessionHandler : SessionHandler
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
            _logger.Error(exception,
                "Exception occurred.");
            await _outboundListener.OnError(exception);
        }

        protected override void ChannelRead0(IChannelHandlerContext context,
            Message message)
        {
            switch (message)
            {
                case Message msg when !string.IsNullOrEmpty(msg?.ContentType()):
                    switch (message.ContentType())
                    {
                        case HeadersValues.AuthRequest:
                            _outboundListener.OnAuthentication().ConfigureAwait(false);
                            break;
                        case HeadersValues.CommandReply:
                        case HeadersValues.ApiResponse:
                            var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                            var apiResponse = new ApiResponse(commandAsyncEvent.Command.Command,
                                message);
                            commandAsyncEvent.Complete(apiResponse);
                            break;
                        case HeadersValues.TextEventPlain:
                            _outboundListener.OnEventReceived(message);
                            break;
                        case HeadersValues.TextDisconnectNotice:
                            var channel = context.Channel;
                            var address = channel.RemoteAddress;

                            _outboundListener.OnDisconnectNotice(message,
                               address).ConfigureAwait(false);
                            break;
                        case HeadersValues.TextRudeRejection:
                            _outboundListener.OnRudeRejection().ConfigureAwait(false);
                            break;
                        default:
                            Console.WriteLine(message.ContentType());
                            // Unexpected freeSwitch message
                            _logger.Warn("Unexpected message content type [{0}]",
                                message.ContentType());
                            break;
                    }
                    break;
                default:
                    // Unexpected freeSwitch message
                    _logger.Warn("Unexpected message [{0}]",
                        message);
                    return;

            }

        }

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            // pass
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}
