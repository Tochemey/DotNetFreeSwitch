using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Events;
using NLog;

namespace ModFreeSwitch.Handlers.outbound {
    public class EslClient : IEventListener {
        private readonly SemaphoreSlim _connectSemaphore = new SemaphoreSlim(0);
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Bootstrap _bootstrap;
        private IChannel _channel;

        private MultithreadEventLoopGroup _eventLoopGroup;

        public EslClient(string address,
            int port,
            string password,
            TimeSpan connectionTimeout) {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = connectionTimeout;
        }

        public EslClient(string address,
            int port,
            string password) {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = new TimeSpan(0, 0, 0, 0, 100);
        }


        public EslClient() : this("localhost", 8021, "ClueCon") {}

        public EslClient(TimeSpan timeout) : this("localhost", 8021, "ClueCon", timeout) {}

        public string Address { get; }
        public bool Authenticated { get; private set; }
        public string Password { get; }
        public int Port { get; }
        public TimeSpan ConnectionTimeout { get; }

        public Task OnEventReceived(EslEvent eslEvent) {
            throw new NotImplementedException();
        }

        public bool CanSend() {
            var handler = (EslClientHandler) _channel.Pipeline.Last();
            Authenticated = handler.Authenticated;
            return Authenticated && IsActive();
        }


        /// <summary>
        ///     ConnectAsync().
        ///     Connect asynchronously to freeSwitch mod_event_socket.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync() {
            _logger.Info("connecting to freeSwitch mod_event_socket...");
            try {
                Initialize();
                _channel = await _bootstrap.ConnectAsync(Address, Port);
                await _connectSemaphore.WaitAsync(ConnectionTimeout);
            }
            finally {
                _connectSemaphore.Release();
            }

            _logger.Info("successfully connected to freeSwitch mod_event_socket.");
        }

        public bool IsActive() {
            return _channel != null && _channel.Active;
        }

        public async Task DisconnectAsync() {
            await _channel.CloseAsync();
            await _eventLoopGroup.ShutdownGracefullyAsync();
        }

        public async Task<bool> Subscribe(string events) {
            if (!CanSend()) return false;
            var handler = (EslClientHandler) _channel.Pipeline.Last();
            var command = new EventCommand(events);
            var reply = await handler.SendCommand(command, _channel);
            return reply.IsOk;
        }

        protected void Initialize() {
            _eventLoopGroup = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();
            _bootstrap.Group(_eventLoopGroup);
            _bootstrap.Channel<TcpSocketChannel>();
            _bootstrap.Option(ChannelOption.SoLinger, 1);
            _bootstrap.Option(ChannelOption.TcpNodelay, true);
            _bootstrap.Option(ChannelOption.SoKeepalive, true);
            _bootstrap.Option(ChannelOption.SoReuseaddr, true);
            _bootstrap.Handler(new EslClientInitializer(Password, this));
        }
    }
}