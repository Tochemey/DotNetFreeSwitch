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

using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using LogLevel = DotNetty.Handlers.Logging.LogLevel;

namespace DotNetFreeSwitch.Handlers.inbound
{
   public class InboundServer
   {
      private readonly ServerBootstrap _bootstrap;
      private readonly MultithreadEventLoopGroup _bossEventLoopGroup;
      private readonly Logger _logger = LogManager.GetCurrentClassLogger();
      private readonly MultithreadEventLoopGroup _workerEventLoopGroup;
      private readonly InboundSession inboundSession;
      private IChannel _channel;

      /// <summary>
      /// Creates an instance of the InboundServer
      /// </summary>
      /// <param name="port">the binding port</param>
      /// <param name="backlog">the number of incoming connections to handle at a go</param>
      /// <param name="inboundSession">the incoming session handler</param>
      public InboundServer(int port,
          int backlog,
          InboundSession inboundSession)
      {
         Port = port;
         Backlog = backlog;
         this.inboundSession = inboundSession;
         _bootstrap = new ServerBootstrap();
         _bossEventLoopGroup = new MultithreadEventLoopGroup(1);
         _workerEventLoopGroup = new MultithreadEventLoopGroup();
      }

      /// <summary>
      /// Creates an instance of the InboundServer
      /// </summary>
      /// <param name="port">the binding port</param>
      /// <param name="inboundSession">the incoming session handler</param>
      /// <returns></returns>
      public InboundServer(int port,
          InboundSession inboundSession) : this(port,
          100,
          inboundSession)
      {
      }

      public int Backlog { get; }
      public int Port { get; }

      /// <summary>
      /// Starts the tcp server
      /// </summary>
      /// <returns></returns>
      public async Task StartAsync()
      {
         Init();
         _channel = await _bootstrap.BindAsync(Port);
      }

      /// <summary>
      /// Stops the tcp server
      /// </summary>
      /// <returns></returns>
      public async Task StopAsync()
      {
         if (_channel != null) await _channel.CloseAsync();
         if (_bossEventLoopGroup != null && _workerEventLoopGroup != null)
         {
            await _bossEventLoopGroup.ShutdownGracefullyAsync();
            await _workerEventLoopGroup.ShutdownGracefullyAsync();
         }
      }


      /// <summary>
      /// Returns true when the server has started and false on the contrary
      /// </summary>
      /// <returns></returns>
      public bool Started()
      {
         return _channel != null && _channel.Active;
      }

      /// <summary>
      /// Initialize the tcp server
      /// </summary>
      protected void Init()
      {
         _bootstrap.Group(_bossEventLoopGroup,
             _workerEventLoopGroup);
         _bootstrap.Channel<TcpServerSocketChannel>();
         _bootstrap.Option(ChannelOption.SoLinger,
             0);
         _bootstrap.Option(ChannelOption.SoBacklog,
             Backlog);
         _bootstrap.Handler(new LoggingHandler(LogLevel.INFO));
         _bootstrap.ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
         {
            var pipeline = channel.Pipeline;
            pipeline.AddLast("FrameDecoder",
                new Codecs.FrameDecoder(true));
            pipeline.AddLast("FrameEncoder",
                new Codecs.FrameEncoder());
            pipeline.AddLast("StringEncoder",
                new StringEncoder());
            pipeline.AddLast("DebugLogging",
                new LoggingHandler(LogLevel.INFO));
            pipeline.AddLast(new InboundSessionHandler(inboundSession));
         }));
         _bootstrap.ChildOption(ChannelOption.SoLinger,
             0);
         _bootstrap.ChildOption(ChannelOption.SoKeepalive,
             true);
         _bootstrap.ChildOption(ChannelOption.TcpNodelay,
             true);
         _bootstrap.ChildOption(ChannelOption.SoReuseaddr,
             true);
      }
   }
}
