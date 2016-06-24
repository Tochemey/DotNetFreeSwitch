using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using ModFreeSwitch.Handlers.outbound;
using ModFreeSwitch.Messages;
using Xunit;

namespace ModFreeSwitch.Test
{
    public class EslTest : IDisposable {
        private string _event;

        public EslTest() {
        }

        [Fact]
        public void OneBodyLineMessageTest() {
            _event = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\Gateways.txt";
            var charBytes = File.ReadAllBytes(_event);
            var message = Unpooled.CopiedBuffer(charBytes);
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(message);
            var buf = channel.ReadInbound<EslMessage>();

            string body = string.Join("", buf.BodyLines);
            Assert.Equal("example.com smsghlocalsip", body);
        }

        [Fact]
        public void HeaderParserTest() {
            string headerLine = "Content-Length: 1754\n";
            string[] parsedHeader = EslHeaderParser.SplitHeader(headerLine);

            Assert.Equal("Content-Length", parsedHeader[0]);
            Assert.Equal("1754", parsedHeader[1]);
        }

        [Fact]
        public void EventParserTest() {
            _event = AppDomain.CurrentDomain.BaseDirectory + "\\Messages\\ChannelProgressEvent.txt";
            var charBytes = File.ReadAllBytes(_event);
            var msg = Unpooled.CopiedBuffer(charBytes);
            // Let us read the file
            var channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(msg);
            var buf = channel.ReadInbound<EslMessage>();
            var bodyLines = buf.BodyLines;

            // Let us parse the first body line
            string[] body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name", body[0]);
            Assert.Equal(true, buf.HasHeader(EslHeaders.ContentLength));
            
        }

        [Fact]
        public void BackgroundJobEventParserTest() {
            string eventData = AppDomain.CurrentDomain.BaseDirectory +
                               "\\Messages\\BackgroundJob.txt";
            byte[] backgroundJobBytes = File.ReadAllBytes(eventData);
            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            EmbeddedChannel channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            EslMessage message = channel.ReadInbound<EslMessage>();
            List<string> bodyLines = message.BodyLines;
            string[] body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name", body[0]);
            Assert.Equal(true, message.HasHeader(EslHeaders.ContentLength));
            string last = bodyLines.Last().TrimEnd('\n');
            Assert.Equal(true, last.Contains("OK"));
            Assert.Equal("+OK b317e530-1991-43d3-a03d-79a460a048c1", last);

        }

        [Fact]
        public void FreeSwitchCommandEncodingTest() {
            AuthCommand authentication = new AuthCommand("ClueCon");
            IChannelHandler[] handlers = new IChannelHandler[2];
            handlers[0] = new EslFrameEncoder();
            handlers[1] = new StringEncoder();
            EmbeddedChannel channel = new EmbeddedChannel(handlers);
            
            channel.WriteOutbound(authentication);
            var buf = channel.ReadOutbound<IByteBuffer>();
            Assert.Equal("auth ClueCon\n\n".Length, buf.ReadableBytes);
        }

        [Fact]
        public void ChannelDataParserTest() {
            string eventData = AppDomain.CurrentDomain.BaseDirectory +
                               "\\Messages\\ChannelData.txt";
            byte[] backgroundJobBytes = File.ReadAllBytes(eventData);
            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            EmbeddedChannel channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            EslMessage message = channel.ReadInbound<EslMessage>();
            Assert.Equal(true, message.HasHeader("Event-Name"));
            Assert.Equal("CHANNEL_DATA", message.Headers["Event-Name"]);
        }

        [Fact]
        public void ChannelDataParserAsCommandReplyTest()
        {
            string eventData = AppDomain.CurrentDomain.BaseDirectory +
                               "\\Messages\\ChannelData.txt";
            byte[] backgroundJobBytes = File.ReadAllBytes(eventData);
            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            EmbeddedChannel channel = new EmbeddedChannel(new EslFrameDecoder());
            channel.WriteInbound(byteBuffer);

            EslMessage message = channel.ReadInbound<EslMessage>();
            CommandReply commandReply = new CommandReply("connect", message);
            Assert.Equal("+OK", commandReply.ReplyText);
            Assert.Equal(true, commandReply.IsOk);
            Assert.Equal("command/reply", commandReply.ContentType);            
        }

        [Fact]
        public void ConnectToFreeSwitchTest() {
            string address = "192.168.74.128";
            string password = "ClueCon";
            int port = 8021;

            OutboundSession client = new OutboundSession(address,port, password);
            client.ConnectAsync().Wait(500);
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
            Assert.Equal(true, client.IsActive());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
            Assert.Equal(true, client.CanSend());
        }

        [Fact]
        public async void SubscribeToEventsTest() {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            OutboundSession client = new OutboundSession(address, port, password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework

            string @event = "plain ALL";
            bool subscribed = await client.SubscribeAsync(@event);

            Assert.Equal(true, subscribed);
        }

        [Fact]
        public async void SendApiTest() {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            OutboundSession client = new OutboundSession(address, port, password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
            const string commandString = "sofia profile external gwlist up";
            ApiResponse response = await client.SendApiAsync(new ApiCommand(commandString));

            Assert.Contains("smsghlocalsip", response.ReplyText);
        }

        [Fact]
        public async void SendBgApiTest() {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            OutboundSession client = new OutboundSession(address, port, password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
            Guid jobId = await client.SendBgApiAsync(new BgApiCommand("status", string.Empty));

            Assert.True(jobId != Guid.Empty);
        }

        [Fact]
        public async void SendCommandTest() {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;

            OutboundSession client = new OutboundSession(address, port, password);
            await client.ConnectAsync();
            Thread.Sleep(100); // this is due to the asynchronous pattern of the framework
   
            BgApiCommand cmd = new BgApiCommand("fsctl", "debug_level 7");
            CommandReply reply = await client.SendCommandAsync(cmd);
            Assert.True(reply.IsOk);
        }

        public void Dispose() {
            _event = null;
        }
    }
}
