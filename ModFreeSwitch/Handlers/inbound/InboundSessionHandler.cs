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
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundSessionHandler : EslSessionHandler {

        private readonly IInboundListener _inboundListener;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InboundSessionHandler(IInboundListener inboundListener) {
            _inboundListener = inboundListener;
        }

        public override async void ExceptionCaught(IChannelHandlerContext context,
            Exception exception) {
            if(!context.Channel.Open) return;
            _logger.Error(exception, "Exception occured.");
            await _inboundListener.OnError(exception);
        }

        public override async void ChannelActive(IChannelHandlerContext context) {
            var channel = context.Channel;
            _logger.Debug(
                "received a new connection from freeswitch {0}. Sending a connect command...",
                channel.RemoteAddress);
            var connectCommand = new ConnectCommand();
            var reply = await SendCommandAsync(connectCommand, channel);
            if (!reply.IsOk) return;
            var connectedCall = new ConnectedCall(new EslEvent(reply.Response, true));
            await _inboundListener.OnConnected(connectedCall, channel);
        }

        public override async void ChannelRead(IChannelHandlerContext context,
            object message) {
            var eslMessage = message as EslMessage;
            var contentType = eslMessage?.ContentType();

            if(string.IsNullOrEmpty(contentType)) return;
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
                await _inboundListener.OnEventReceived(eslMessage);
                return;
            }

            // Handle disconnect/notice message
            if (contentType.Equals(EslHeadersValues.TextDisconnectNotice)) {
                IChannel channel = context.Channel;
                EndPoint address = channel.RemoteAddress;
                await _inboundListener.OnDisconnectNotice(eslMessage, address);
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

    }
}
