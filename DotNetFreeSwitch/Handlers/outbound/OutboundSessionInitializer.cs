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

using DotNetFreeSwitch.Codecs;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace DotNetFreeSwitch.Handlers.outbound
{
    public class OutboundSessionInitializer : ChannelInitializer<ISocketChannel>
    {
        public OutboundSessionInitializer(IOutboundListener outboundListener)
        {
            OutboundListener = outboundListener;
        }

        public IOutboundListener OutboundListener { get; }

        protected override void InitChannel(ISocketChannel channel)
        {
            // get the channel pipeline
            var pipeline = channel.Pipeline;
            pipeline.AddLast("FrameDecoder",
                new FrameDecoder());
            pipeline.AddLast("FrameEncoder",
                new FrameEncoder());
            pipeline.AddLast("StringEncoder",
                new StringEncoder());
            pipeline.AddLast("DebugLogging",
                new LoggingHandler(LogLevel.INFO));
            pipeline.AddLast(new OutboundSessionHandler(OutboundListener));
        }
    }
}