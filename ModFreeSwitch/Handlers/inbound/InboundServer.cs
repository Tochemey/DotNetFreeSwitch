using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using LogLevel = DotNetty.Handlers.Logging.LogLevel;

namespace ModFreeSwitch.Handlers.inbound {
    public class InboundServer {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private ServerBootstrap _bootstrap;
        private readonly MultithreadEventLoopGroup _bossEventLoopGroup;
        private IChannel _channel;
        private readonly MultithreadEventLoopGroup _workerEventLoopGroup;

        public InboundServer(int port,
            int backlog) {
            Port = port;
            Backlog = backlog;
            _bossEventLoopGroup = new MultithreadEventLoopGroup(1);
            _workerEventLoopGroup = new MultithreadEventLoopGroup();
        }

        public InboundServer(int port) : this(port, 100) {}

        public int Port { get; }
        public int Backlog { get; }

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
            _bootstrap.Option(ChannelOption.SoLinger, 1);
            _bootstrap.Option(ChannelOption.SoBacklog, Backlog);
            _bootstrap.Handler(new LoggingHandler(LogLevel.INFO));
            _bootstrap.ChildHandler(new InboundSessionInitializer(new InboundSession()));
        }
    }
}