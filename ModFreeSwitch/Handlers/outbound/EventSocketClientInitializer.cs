using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Codecs;

namespace ModFreeSwitch.Handlers.outbound {
    /// <summary>
    ///     Helps to bootstrap all the necessary codecs and handlers needed by the <see cref="EventSocketClient" /> to function
    ///     properly
    /// </summary>
    public class EventSocketClientInitializer : ChannelInitializer<ISocketChannel> {
        public EventSocketClientInitializer(EventSocketClientHandler handler) { Handler = handler; }

        /// <summary>
        ///     Request handler
        /// </summary>
        public EventSocketClientHandler Handler { get; private set; }

        protected override void InitChannel(ISocketChannel channel) {
            // Let us get the channel pipeline
            IChannelPipeline pipeline = channel.Pipeline;
            pipeline.AddLast("decoder", new EventSocketMessageDecoder(8192));
            pipeline.AddLast("encoder", new EventSocketMessageEncoder());
            pipeline.AddLast("handler", Handler);
        }
    }
}