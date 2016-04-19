using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using NLog;

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     Helps to encode the command that will be sent to freeSwitch via the mod_event_socket
    /// </summary>
    public class EslEncoder : MessageToMessageEncoder<BaseCommand> {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _messageEndString = "\n\n";

        protected override void Encode(IChannelHandlerContext context, BaseCommand message, List<object> output) {
            if (message == null) return;
            // Let us get the string representation of the message sent
            string msg = message.ToString().Trim();
            if (string.IsNullOrEmpty(msg)) return;
            if (!msg.Trim().EndsWith(_messageEndString)) msg += _messageEndString;
            if (logger.IsDebugEnabled) logger.Debug("Encoded message sent [{0}]", msg.Trim());
            output.Add(msg);
        }
    }
}