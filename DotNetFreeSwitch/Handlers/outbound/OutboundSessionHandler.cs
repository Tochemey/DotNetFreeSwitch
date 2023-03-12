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

      /// <summary>
      /// Initialize the OutboundSessionHandler
      /// </summary>
      /// <param name="outboundListener"></param>
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

      public override async void ChannelRead(IChannelHandlerContext ctx, object message)
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
            case HeadersValues.AuthRequest:
               await _outboundListener.OnAuthentication();
               break;
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
               _outboundListener.OnEventReceived(msg);
               break;
            case HeadersValues.TextDisconnectNotice:
               var channel = ctx.Channel;
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
      }

      public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
   }
}
