using System;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class EslEventArgs : EventArgs {
        public EslEventArgs(IChannelHandlerContext context,
            EslMessage message) {
            Context = context;
            Message = message;
        }

        public IChannelHandlerContext Context { get; private set; }
        public EslMessage Message { get; private set; }
    }
}