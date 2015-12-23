using System;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class EventSocketArgs : EventArgs {
        public EventSocketArgs(IChannelHandlerContext context, EventSocketMessage message) {
            Context = context;
            Message = message;
        }

        public IChannelHandlerContext Context { get; private set; }
        public EventSocketMessage Message { get; private set; }
    }
}