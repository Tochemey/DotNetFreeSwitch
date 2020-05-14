using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Codecs;
using Core.Commands;
using Core.Common;
using Core.Events;
using Core.Handlers.inbound;
using Core.Handlers.outbound;
using Core.Messages;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Embedded;
using Microsoft.Extensions.FileProviders;
using NLog;
using Xunit;

namespace ModFreeSwitch.Test
{
    public class Parser : IDisposable
    {
        [Fact]
        public void BackgroundJobEventParserTest()
        {
            var eventData = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/BackgroundJob.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new FrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<FsMessage>();
            var bodyLines = message.BodyLines;
            var body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name",
                body[0]);
            Assert.True(message.HasHeader(Headers.ContentLength));
            var last = bodyLines.Last().TrimEnd('\n');
            Assert.Contains("OK", last);
            Assert.Equal("+OK b317e530-1991-43d3-a03d-79a460a048c1",
                last);
        }

        [Fact]
        public void ChannelDataParserAsCommandReplyTest()
        {
            var eventData = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/ChannelData.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new FrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<FsMessage>();
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
            var eventData = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/ChannelData.txt";
            var backgroundJobBytes = File.ReadAllBytes(eventData);
            var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
            var channel = new EmbeddedChannel(new FrameDecoder());
            channel.WriteInbound(byteBuffer);

            var message = channel.ReadInbound<FsMessage>();
            Assert.True(message.HasHeader("Event-Name"));
            Assert.Equal("CHANNEL_DATA",
                message.Headers["Event-Name"]);
        }

        
        [Fact]
        public void EventParserTest()
        {
            var @event = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/ChannelProgressEvent.txt";
            var charBytes = File.ReadAllBytes(@event);
            var msg = Unpooled.CopiedBuffer(charBytes);
            // Let us read the file
            var channel = new EmbeddedChannel(new FrameDecoder());
            channel.WriteInbound(msg);
            var buf = channel.ReadInbound<FsMessage>();
            var bodyLines = buf.BodyLines;

            // Let us parse the first body line
            var body = EslHeaderParser.SplitHeader(bodyLines.First());
            Assert.Equal("Event-Name",
                body[0]);
            Assert.True(buf.HasHeader(Headers.ContentLength));
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
        public void OneBodyLineMessageTest()
        {
            var @event = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/Gateways.txt";
            var charBytes = File.ReadAllBytes(@event);
            var message = Unpooled.CopiedBuffer(charBytes);
            var channel = new EmbeddedChannel(new FrameDecoder());
            channel.WriteInbound(message);
            var buf = channel.ReadInbound<FsMessage>();

            var body = string.Join("",
                buf.BodyLines);
            Assert.Equal("example.com smsghlocalsip",
                body);
        }
        
        public void Dispose()
        {
        }
    }
}
