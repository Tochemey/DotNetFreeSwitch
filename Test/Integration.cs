using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Codecs;
using Core.Commands;
using Core.Common;
using Core.Events;
using Core.Handlers.inbound;
using Core.Handlers.outbound;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;
using Xunit;

namespace Test
{
    public class Integration : IDisposable
    {
        public void Dispose() { _event = null; }

        private string _event;

        public class DefaultInboundSession : InboundSession
        {
            private const string audioFile = "https://s3.amazonaws.com/plivocloud/Trumpet.mp3";

            protected override Task HandleEvents(FsEvent @event,
                EventType eventType)
            {
                Console.WriteLine(@event);
                return Task.CompletedTask;
            }

            protected override Task PreHandleAsync() { return Task.CompletedTask; }

            protected override async Task HandleAsync() { await PlayAsync(audioFile); }
        }
        

        [Fact]
        public async void ConnectToFreeSwitchTest()
        {
            var address = "192.168.74.128";
            var password = "ClueCon";
            var port = 8021;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Assert.True(client.IsActive());
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework

            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
            Assert.True(client.IsSessionReady());
        }



        [Fact]
        public void FreeSwitchCommandEncodingTest()
        {
            var authentication = new AuthCommand("ClueCon");
            var handlers = new IChannelHandler[2];
            handlers[0] = new FrameEncoder();
            handlers[1] = new StringEncoder();
            var channel = new EmbeddedChannel(handlers);

            channel.WriteOutbound(authentication);
            var buf = channel.ReadOutbound<IByteBuffer>();
            Assert.Equal("auth ClueCon\n\n".Length,
                buf.ReadableBytes);
        }



        [Fact]
        public async void InboundModeTest()
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;
            const int ServerPort = 10000;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework

            var inboundServer = new InboundServer(ServerPort,
                new DefaultInboundSession());
            await inboundServer.StartAsync();
            var callCommand = "{ignore_early_media=false,originate_timeout=120}sofia/gateway/smsghlocalsip/233247063817 &socket(192.168.74.1:10000 async full)";

            var jobId = await client.SendBgApiAsync(new BgApiCommand("originate",
                callCommand));
            Assert.True(jobId != Guid.Empty);
        }



        [Fact]
        public async void SendApiTest()
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
            const string commandString = "sofia profile external gwlist up";
            var response = await client.SendApiAsync(new ApiCommand(commandString));

            Assert.Contains("smsghlocalsip",
                response.ReplyText);
        }

        [Fact]
        public async void SendBgApiTest()
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
            var jobId = await client.SendBgApiAsync(new BgApiCommand("status",
                string.Empty));

            Assert.True(jobId != Guid.Empty);
        }

        [Fact]
        public async void SendCommandTest()
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework

            var cmd = new BgApiCommand("fsctl",
                "debug_level 7");
            var reply = await client.SendCommandAsync(cmd);
            Assert.True(reply.IsOk);
        }

        [Fact]
        public async void SubscribeToEventsTest()
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            var client = new OutboundSession(address,
                port,
                password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework

            var @event = "plain ALL";
            var subscribed = await client.SubscribeAsync(@event);

            Assert.True(subscribed);
        }
    }
}
