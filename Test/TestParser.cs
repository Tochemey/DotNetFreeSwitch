using System;
using System.IO;
using System.Linq;
using DotNetFreeSwitch.Codecs;
using DotNetFreeSwitch.Messages;
using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Xunit;

namespace Test
{
   public class TestParser : IDisposable
   {
      [Fact]
      public void BackgroundJobEventParserTest()
      {
         var eventData = AppDomain.CurrentDomain.BaseDirectory + @"/Messages/BackgroundJob.txt";
         var backgroundJobBytes = File.ReadAllBytes(eventData);
         var byteBuffer = Unpooled.CopiedBuffer(backgroundJobBytes);
         var channel = new EmbeddedChannel(new FrameDecoder());
         channel.WriteInbound(byteBuffer);

         var message = channel.ReadInbound<Message>();
         var bodyLines = message.BodyLines;
         var body = HeaderParser.SplitHeader(bodyLines.First());
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

         var message = channel.ReadInbound<Message>();
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

         var message = channel.ReadInbound<Message>();
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
         var buf = channel.ReadInbound<Message>();
         var bodyLines = buf.BodyLines;

         // Let us parse the first body line
         var body = HeaderParser.SplitHeader(bodyLines.First());
         Assert.Equal("Event-Name",
             body[0]);
         Assert.True(buf.HasHeader(Headers.ContentLength));
      }

      [Fact]
      public void HeaderParserTest()
      {
         var headerLine = "Content-Length: 1754\n";
         var parsedHeader = HeaderParser.SplitHeader(headerLine);

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
         var buf = channel.ReadInbound<Message>();

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
