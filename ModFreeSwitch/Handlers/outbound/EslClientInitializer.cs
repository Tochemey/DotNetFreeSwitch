using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Codecs;

namespace ModFreeSwitch.Handlers.outbound {
    public class EslClientInitializer : ChannelInitializer<ISocketChannel> {
        protected override void InitChannel(ISocketChannel channel) {

            // get the channel pipeline
            IChannelPipeline pipeline = channel.Pipeline;
            pipeline.AddLast("EslFrameDecoder", new EslFrameDecoder());
            pipeline.AddLast("EslFrameEncoder", new EslFrameEncoder());
            pipeline.AddLast("StringEncoder", new StringEncoder());
        }
    }
}