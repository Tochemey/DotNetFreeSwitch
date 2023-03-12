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
using DotNetFreeSwitch.Commands;
using DotNetFreeSwitch.Events;
using DotNetFreeSwitch.Messages;
using DotNetty.Transport.Channels;
using NLog;

namespace DotNetFreeSwitch.Handlers.inbound
{
   public class InboundSessionHandler : SessionHandler
   {
      private readonly IInboundListener _inboundListener;
      private readonly Logger _logger = LogManager.GetCurrentClassLogger();

      public InboundSessionHandler(IInboundListener inboundListener)
      {
         _inboundListener = inboundListener;
      }

      public override async void ExceptionCaught(IChannelHandlerContext context,
          Exception exception)
      {
         if (!context.Channel.Open) return;
         _logger.Error(exception,
             "Exception occurred.");
         await _inboundListener.OnError(exception);
      }

      public override async void ChannelActive(IChannelHandlerContext context)
      {
         var channel = context.Channel;
         _logger.Debug("received a new connection from freeswitch {0}. Sending a connect command...",
             channel.RemoteAddress);
         var connectCommand = new ConnectCommand();
         var reply = await SendCommandAsync(connectCommand,
             channel);
         if (!reply.IsOk) return;
         var connectedCall = new InboundCall(new Event(reply.Response,
             true));
         await _inboundListener.OnConnected(connectedCall,
             channel);
      }

      public override async void ChannelRead(IChannelHandlerContext context,
          object message)
      {
         // assert that we do really parse the freeswitch message
         // if not log an warning and return
         if (!(message is Message))
         {
            // Unexpected freeSwitch message
            _logger.Warn("Unexpected message [{0}]",
                message);
            return;
         }

         // cast the message to the Message type
         var msg = (Message)message;
         // make sure we do have a content type
         if (string.IsNullOrEmpty(msg?.ContentType()))
         {
            // Unexpected freeSwitch message
            _logger.Warn("Unexpected message [{0}]",
                message);
            return;
         }

         // add a debug logging in case of debugging
         _logger.Debug($"freeswitch message type: {msg.ContentType()}");

         switch (msg.ContentType())
         {
            case HeadersValues.CommandReply:
               var commandAsyncEvent = CommandAsyncEvents.Dequeue();
               var commandReply = new CommandReply(commandAsyncEvent.Command.CommandName, msg);
               commandAsyncEvent.Complete(commandReply);
               break;
            case HeadersValues.ApiResponse:
               var apiAsyncEvent = CommandAsyncEvents.Dequeue();
               var apiResponse = new ApiResponse(apiAsyncEvent.Command.CommandName,
                   msg);
               apiAsyncEvent.Complete(apiResponse);
               break;
            case HeadersValues.TextEventPlain:
               _inboundListener.OnEventReceived(msg);
               break;
            case HeadersValues.TextDisconnectNotice:
               var channel = context.Channel;
               var address = channel.RemoteAddress;

               await _inboundListener.OnDisconnectNotice(msg,
                   address);
               break;
            case HeadersValues.TextRudeRejection:
               await _inboundListener.OnRudeRejection();
               break;
            default:
               // Unexpected freeSwitch message
               _logger.Warn("Unexpected message content type [{0}]",
                   msg.ContentType());
               break;
         }

      }
   }
}
