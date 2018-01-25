using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;
using ModFreeSwitch.Codecs;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Handlers.inbound;
using ModFreeSwitch.Handlers.outbound;
using ModFreeSwitch.Messages;
using NLog;
using Xunit;

namespace ModFreeSwitch.Test
{
    public class EslTest : IDisposable
    {
        public void Dispose() { _event = null; }

        private string _event;

        public class DefaultInboundSession : InboundSession
        {
            private const string audioFile = "https://s3.amazonaws.com/plivocloud/Trumpet.mp3";
            private readonly Logger _logger = LogManager.GetCurrentClassLogger();

            protected override Task HandleEvents(EslEvent @event,
                EslEventType eventType)
            {
                _logger.Debug(@event);
                return Task.CompletedTask;
            }

            protected override Task PreHandleAsync() { return Task.CompletedTask; }

            protected override async Task HandleAsync() { await PlayAsync(audioFile); }
        }

        [Fact]
        public void BackgroundJobEventParserTest()
        {
            var eventData = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\BackgroundJob.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<EslMessage>();
            var bodyLines = message.BodyLines;
            var body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name",
                body[0]);
            Assert.True(message.HasHeader(EslHeaders.ContentLength));
            var last = bodyLines.Last().TrimEnd('\n');
            Assert.Contains("OK", last);
            Assert.Equal("+OK b317e530-1991-43d3-a03d-79a460a048c1",
                last);
        }

        [Fact]
        public void ChannelDataParserAsCommandReplyTest()
        {
            var eventData = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\ChannelData.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<EslMessage>();
            var commandReply = new CommandReply("connect",
                message);
            Assert.Equal("+OK",
                commandReply.ReplyText);
            Assert.True(commandReply.IsOk);
            Assert.Equal("command/reply",
                commandReply.ContentType);
        }

        [Fact]
        public void ChannelDataParserTest()
        {
            var eventData = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\ChannelData.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<EslMessage>();
            Assert.True(message.HasHeader("Event-Name"));
            Assert.Equal("CHANNEL_DATA",
                message.Headers["Event-Name"]);
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

            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
            Assert.True(client.CanSend());
        }

        [Fact]
        public void EventParserTest()
        {
            _event = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\ChannelProgressEvent.txt";
            var charBytes = File.ReadAllBytes(_event);
            var msg = Unpooled.CopiedBuffer(charBytes);
            // Let us read the file
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(msg);
            var buf = channel.ReadInbound<EslMessage>();
            var bodyLines = buf.BodyLines;

            // Let us parse the first body line
            var body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name",
                body[0]);
            Assert.True(buf.HasHeader(EslHeaders.ContentLength));
        }

        [Fact]
        public void FreeSwitchCommandEncodingTest()
        {
            var authentication = new AuthCommand("ClueCon");
            var handlers = new IChannelHandler[2];
            handlers[0] = new EslFrameEncoder();
            handlers[1] = new StringEncoder();
            var channel = new EmbeddedChannel(handlers);

            channel.WriteOutbound(authentication);
            var buf = channel.ReadOutbound<IByteBuffer>();
            Assert.Equal("auth ClueCon\n\n".Length,
                buf.ReadableBytes);
        }

        [Fact]
        public void HeaderParserTest()
        {
            var headerLine = "Content-Length: 1754\n";
            var parsedHeader = EslHeaderParser.SplitHeader(headerLine);

            Assert.Equal("Content-Length",
                parsedHeader[0]);
            Assert.Equal("1754",
                parsedHeader[1]);
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
        public void OneBodyLineMessageTest()
        {
            _event = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\Gateways.txt";
            var charBytes = File.ReadAllBytes(_event);
            var message = Unpooled.CopiedBuffer(charBytes);
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(message);
            var buf = channel.ReadInbound<EslMessage>();

            var body = string.Join("",
                buf.BodyLines);
            Assert.Equal("example.com smsghlocalsip",
                body);
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