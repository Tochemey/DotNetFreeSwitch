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
using Core.Messages;
using DotNetty.Transport.Channels;
using NLog;

namespace Core.Handlers.outbound
{
    /// <summary>
    ///     OutboundSessionHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class OutboundSessionHandler : EslSessionHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IOutboundListener _outboundListener;

        public OutboundSessionHandler(IOutboundListener outboundListener) { _outboundListener = outboundListener; }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception)
        {
            _logger.Error(exception,
                "Exception occured.");
            await _outboundListener.OnError(exception);
        }

        public override async void ChannelRead(IChannelHandlerContext context,
            object message)
        {
            switch (message)
            {
                case FsMessage msg when !string.IsNullOrEmpty(msg?.ContentType()):
                    switch (msg.ContentType())
                    {
                        case HeadersValues.AuthRequest:
                            await _outboundListener.OnAuthentication();
                            break;
                        case HeadersValues.CommandReply:
                        case HeadersValues.ApiResponse:
                            var commandAsyncEvent = CommandAsyncEvents.Dequeue();
                            var apiResponse = new ApiResponse(commandAsyncEvent.Command.Command,
                                msg);
                            commandAsyncEvent.Complete(apiResponse);
                            break;
                        case HeadersValues.TextEventPlain:
                            _outboundListener.OnEventReceived(msg);
                            break;
                        case HeadersValues.TextDisconnectNotice:
                            var channel = context.Channel;
                            var address = channel.RemoteAddress;

                            await _outboundListener.OnDisconnectNotice(msg,
                                address);
                            break;
                        case HeadersValues.TextRudeRejection:
                            await _outboundListener.OnRudeRejection();
                            break;
                        default:
                            // Unexpected freeSwitch message
                            _logger.Warn("Unexpected message content type [{0}]",
                                msg.ContentType());
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
    }
}