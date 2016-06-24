using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Codecs;

namespace ModFreeSwitch.Handlers.outbound {
    public class OutboundSessionInitializer : ChannelInitializer<ISocketChannel> {
        public OutboundSessionInitializer(
            IOutboundListener outboundListener) {
            OutboundListener = outboundListener;
        }

        public IOutboundListener OutboundListener { get; }

        protected override void InitChannel(ISocketChannel channel) {
            // get the channel pipeline
            var pipeline = channel.Pipeline;
            pipeline.AddLast("EslFrameDecoder", new EslFrameDecoder());
            pipeline.AddLast("EslFrameEncoder", new EslFrameEncoder());
            pipeline.AddLast("StringEncoder", new StringEncoder());
            pipeline.AddLast("DebugLogging", new LoggingHandler(LogLevel.INFO));
            pipeline.AddLast(new OutboundSessionHandler(OutboundListener));
        }
    }
}