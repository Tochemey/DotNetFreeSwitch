using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Codecs;

namespace ModFreeSwitch.Handlers.outbound {
    public class EslClientInitializer : ChannelInitializer<ISocketChannel> {
        public EslClientInitializer(string password,
            IEventListener eventListener) {
            Password = password;
            EventListener = eventListener;
        }

        public string Password { get; }
        public IEventListener EventListener { get; }

        protected override void InitChannel(ISocketChannel channel) {
            // get the channel pipeline
            var pipeline = channel.Pipeline;
            pipeline.AddLast("EslFrameDecoder", new EslFrameDecoder());
            pipeline.AddLast("EslFrameEncoder", new EslFrameEncoder());
            pipeline.AddLast("StringEncoder", new StringEncoder());
            pipeline.AddLast(new EslClientHandler(Password, EventListener));
        }
    }
}