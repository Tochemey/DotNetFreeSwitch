using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using NLog;

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     Helps to encode the command that will be sent to freeSwitch via the mod_event_socket. All it does is to append
    ///     double Line Feed character at the end every command that goes against freeSwitch.
    /// </summary>
    public sealed class EslFrameEncoder : MessageToMessageEncoder<BaseCommand> {
        private const string MessageEndString = "\n\n";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Encoding encoding;

        public EslFrameEncoder(Encoding encoding) {
            this.encoding = encoding;
        }

        public EslFrameEncoder() : this(Encoding.GetEncoding(0)) {}

        public override bool IsSharable {
            get { return true; }
        }

        protected override void Encode(IChannelHandlerContext context,
            BaseCommand message,
            List<object> output) {
            if (message == null) return;
            // Let us get the string representation of the message sent
            if (string.IsNullOrEmpty(message.ToString())) return;
            var msg = message.ToString()
                .Trim();

            if (!msg.Trim()
                .EndsWith(MessageEndString)) msg += MessageEndString;
            if (logger.IsDebugEnabled)
                logger.Debug("Encoded message sent [{0}]", msg.Trim());

            output.Add(ByteBufferUtil.EncodeString(context.Allocator, msg, encoding));
        }
    }
}