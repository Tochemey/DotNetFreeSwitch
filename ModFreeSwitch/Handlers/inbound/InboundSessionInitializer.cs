using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Codecs;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundSessionInitializer : ChannelInitializer<ISocketChannel>
    {
        public InboundSessionInitializer(IInboundListener inboundListener) {
            InboundListener = inboundListener;
        }

        public IInboundListener InboundListener { get; }
        protected override void InitChannel(ISocketChannel channel) {
            // get the channel pipeline
            var pipeline = channel.Pipeline;
            pipeline.AddLast("EslFrameDecoder", new EslFrameDecoder(true));
            pipeline.AddLast("EslFrameEncoder", new EslFrameEncoder());
            pipeline.AddLast("StringEncoder", new StringEncoder());
            pipeline.AddLast("DebugLogging", new LoggingHandler(LogLevel.INFO));
            pipeline.AddLast(new InboundSessionHandler(InboundListener));
        }
    }
}