using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using LogLevel = DotNetty.Handlers.Logging.LogLevel;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundServer {
        private readonly MultithreadEventLoopGroup _bossEventLoopGroup;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly MultithreadEventLoopGroup _workerEventLoopGroup;
        private readonly InboundSession inboundSession;
        private ServerBootstrap _bootstrap;
        private IChannel _channel;

        public InboundServer(int port,
            int backlog,
            InboundSession inboundSession) {
            Port = port;
            Backlog = backlog;
            this.inboundSession = inboundSession;
            _bossEventLoopGroup = new MultithreadEventLoopGroup(1);
            _workerEventLoopGroup = new MultithreadEventLoopGroup();
        }

        public InboundServer(int port,
            InboundSession inboundSession) : this(port, 100, inboundSession) {}

        public int Backlog { get; }
        public int Port { get; }

        public async Task StartAsync() {
            Init();
            _channel = await _bootstrap.BindAsync(Port);
        }

        public async Task StopAsync() {
            if (_channel != null) await _channel.CloseAsync();
            if (_bossEventLoopGroup != null && _workerEventLoopGroup != null) {
                await _bossEventLoopGroup.ShutdownGracefullyAsync();
                await _workerEventLoopGroup.ShutdownGracefullyAsync();
            }
        }

        protected void Init() {
            _bootstrap = new ServerBootstrap();
            _bootstrap.Group(_bossEventLoopGroup, _workerEventLoopGroup);
            _bootstrap.Channel<TcpServerSocketChannel>();
            _bootstrap.Option(ChannelOption.SoLinger, 0);
            _bootstrap.Option(ChannelOption.SoBacklog, Backlog);
            _bootstrap.Handler(new LoggingHandler(LogLevel.INFO));
            _bootstrap.ChildHandler(new InboundSessionInitializer(inboundSession));
            _bootstrap.ChildOption(ChannelOption.SoLinger, 0);
            _bootstrap.ChildOption(ChannelOption.SoKeepalive, true);
            _bootstrap.ChildOption(ChannelOption.TcpNodelay, true);
            _bootstrap.ChildOption(ChannelOption.SoReuseaddr, true);
        }
    }
}