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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DotNetFreeSwitch.Codecs;
using DotNetFreeSwitch.Commands;
using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Events;
using DotNetFreeSwitch.Messages;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;

namespace DotNetFreeSwitch.Handlers.outbound
{
    /// <summary>
    /// OutboundSession is used to connect to freeswitch mod_event_socket in inbound mode.
    /// </summary>
    /// <see cref="https://developer.signalwire.com/freeswitch/FreeSWITCH-Explained/Modules/mod_event_socket_1048924#21-inbound-mode"/>
    public class OutboundSession : IOutboundListener
    {
        private readonly Bootstrap _bootstrap;

        private readonly MultithreadEventLoopGroup _eventLoopGroup;

        private readonly Subject<EventStream> _eventReceived = new Subject<EventStream>();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IChannel _channel;

        /// <summary>
        /// Creates an instance of OutboundSession. One need to make sure to have a valid
        /// freeswitch mod_event_socket configuration in place.
        /// </summary>
        /// <param name="address">the freeswitch host address</param>
        /// <param name="port">the port</param>
        /// <param name="password">the password</param>
        /// <param name="connectionTimeout">the connection timeout.</param>
        public OutboundSession(string address,
            int port,
            string password,
            TimeSpan connectionTimeout)
        {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = connectionTimeout;
            _eventLoopGroup = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();

            Initialize();
        }

        /// <summary>
        /// Creates an instance of OutboundSession.
        /// </summary>
        /// <param name="address">the freeswitch host address</param>
        /// <param name="port">the port</param>
        /// <param name="password"the password></param>
        public OutboundSession(string address,
            int port,
            string password)
        {
            Address = address;
            Port = port;
            Password = password;
            ConnectionTimeout = new TimeSpan(0,
                0,
                0,
                0,
                1000);
            _eventLoopGroup = new MultithreadEventLoopGroup();
            _bootstrap = new Bootstrap();

            Initialize();
        }

        /// <summary>
        /// Creates an instance of OutboundSession with the default freeswitch mod_event_socket
        /// inbound mode credentials.
        /// </summary>
        public OutboundSession() : this("localhost",
            8021,
            "ClueCon")
        {
        }

        /// <summary>
        /// Creates an instance of OutboundSession with the default freeswitch mod_event_socket
        /// inbound mode credentials.
        /// </summary>
        /// <param name="timeout">the connection timeout</param>
        public OutboundSession(TimeSpan timeout) : this("localhost",
            8021,
            "ClueCon",
            timeout)
        {
        }

        /// <summary>
        /// Address returns the OutboundSession address
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// States whether the OutboundSession is authenticated or not
        /// </summary>
        public bool Authenticated { get; private set; }

        /// <summary>
        /// Returns the connection timeout used to connect to freeswitch
        /// </summary>
        public TimeSpan ConnectionTimeout { get; }

        /// <summary>
        /// Returns the password used to connect to freeswitch event_socket in inbound mode
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Returns the port used to connect to freeswitch
        /// </summary>
        public int Port { get; }

        public IObservable<EventStream> EventReceived => _eventReceived.AsObservable();

        /// <summary>
        /// OnAuthentication is triggered when an authentication header is received
        /// </summary>
        /// <returns></returns>
        public async Task OnAuthentication()
        {
            await AuthenticateAsync();
        }

        /// <summary>
        /// OnDisconnectNotice is triggered when a disconnection is received
        /// </summary>
        /// <param name="message">the disconnection message</param>
        /// <param name="channelEndPoint">the tcp channel</param>
        /// <returns></returns>
        public async Task OnDisconnectNotice(Message message,
            EndPoint channelEndPoint)
        {
            _logger.Debug("received disconnection message : {0}",
                message);
            _logger.Warn("channel {0} disconnected",
                channelEndPoint);
            await CleanUpAsync();
        }

        /// <summary>
        /// OnError is triggered when an error occurred
        /// </summary>
        /// <param name="exception">the error</param>
        /// <returns></returns>
        public async Task OnError(Exception exception)
        {
            switch (exception)
            {
                // disconnect when we have encountered system related errors
                case DecoderException _:
                    _logger.Warn($"Encountered an issue during encoding: {exception}. shutting down...");
                    await DisconnectAsync();
                    return;
                case SocketException _:
                    _logger.Warn($"Encountered an issue on the channel: {exception}. shutting down...");
                    await DisconnectAsync();
                    return;
                default:
                    _logger.Error($"Encountered an issue : {exception}");
                    break;
            }
        }

        /// <summary>
        /// OnEventReceived is executed whenever an freeswitch event is emitted on the wire
        /// </summary>
        /// <param name="message">the event message</param>
        public void OnEventReceived(Message message)
        {
            try
            {
                var eslEvent = new FsEvent(message);
                var eventType = EnumExtensions.Parse<EventType>(eslEvent.EventName);
                _eventReceived.OnNext(new EventStream(eslEvent,
                    eventType));
            }
            catch (Exception exception)
            {
                _logger.Warn($"Encountered an issue on the channel: {exception}.");
                _eventReceived.OnError(exception);
            }
        }

        /// <summary>
        /// OnRudeRejection is triggered when rude/rejection is received
        /// </summary>
        /// <returns></returns>
        public async Task OnRudeRejection()
        {
            _logger.Warn("channel {0} received rude/rejection",
                _channel.RemoteAddress);
            await CleanUpAsync();
        }

        /// <summary>
        /// Returns true when the session is authenticated and active
        /// </summary>
        /// <returns></returns>
        public bool IsSessionReady()
        {
            return Authenticated && IsActive();
        }

        /// <summary>
        /// Free up resources asynchronously. This should be used when shutting down the session.
        /// It is automatically called when disconnect notice and rude rejection is received
        /// </summary>
        /// <returns></returns>
        public async Task CleanUpAsync()
        {
            if (_eventLoopGroup != null) await _eventLoopGroup.ShutdownGracefullyAsync();
        }

        /// <summary>
        /// Connects to freeswitch mod_event_socket asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            _logger.Info("connecting to freeSwitch mod_event_socket...");
            _channel = await _bootstrap.ConnectAsync(Address,
                Port);
            _logger.Info("successfully connected to freeSwitch mod_event_socket.");
        }

        /// <summary>
        /// Disconnects from freeswitch mod_event_socket asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_eventLoopGroup != null) await _eventLoopGroup.ShutdownGracefullyAsync();
        }

        /// <summary>
        /// Returns true when the session is active and false on the contrary
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsActive()
        {
            return _channel != null && _channel.Active;
        }


        /// <summary>
        /// Sends api command asynchronously and returns an api response.
        /// </summary>
        /// <param name="apiCommand">the api command</param>
        /// <returns>the api response</returns>
        public async Task<ApiResponse> SendApiAsync(ApiCommand apiCommand)
        {
            if (!IsSessionReady()) return null;
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            var response = await handler.SendApiAsync(apiCommand,
                _channel);
            return response;
        }

        /// <summary>
        /// Sends a background api command and returns the UUID attached to it
        /// </summary>
        /// <param name="bgApiCommand">the background api command</param>
        /// <returns>the UUID of the command sent</returns>
        public async Task<Guid> SendBgApiAsync(BgApiCommand bgApiCommand)
        {
            if (!IsSessionReady()) return Guid.Empty;
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            return await handler.SendBgApiAsync(bgApiCommand,
                _channel);
        }

        /// <summary>
        /// Sends a command to freeswitch and returns a command reply
        /// </summary>
        /// <param name="command">the command</param>
        /// <returns>the command reply</returns>
        public async Task<CommandReply> SendCommandAsync(BaseCommand command)
        {
            if (!IsSessionReady()) return null;
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command,
                _channel);
            return reply;
        }

        /// <summary>
        /// Subscribes to freeswitch event asynchronously and returns true when successful and false on the contrary.
        /// Example: plain CHANNEL_HANGUP CHANNEL_HANGUP_COMPLETE 
        /// </summary>
        /// <example></example>
        /// <param name="events">the event string to subscribe to</param>
        /// <returns>true or false</returns>
        public async Task<bool> SubscribeAsync(string events)
        {
            if (!IsSessionReady()) return false;
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            // let us perform some validation on the events string
            if (!events.StartsWith("plain"))
            {
                throw new ArgumentException("invalid events string: events string must start with the keyword plain followed by the list of freeswitch events space separated");
            }
            var command = new EventCommand(events);
            var reply = await handler.SendCommandAsync(command,
                _channel);
            return reply.IsOk;
        }

        /// <summary>
        ///  Subscribes to a list of freeswitch events asynchronously.
        /// </summary>
        /// <param name="events">the list of freeswitch events</param>
        /// <returns>true when successful and false on the contrary</returns>/
        public async Task<bool> SubscribeAsync(HashSet<string> events)
        {
            // check whether the session is ready or not
            // return false when the session is not ready
            if (!IsSessionReady()) return false;
            // grab the underlying tcp channel handler
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            // let us flatten the event list
            var eventStr = string.Join(" ", events);
            // create the event command
            var eventCmd = $"plain {eventStr}";
            var command = new EventCommand(eventCmd);
            var reply = await handler.SendCommandAsync(command,
                _channel);
            return reply.IsOk;
        }

        /// <summary>
        /// Sends an authentication command to freeswitch
        /// </summary>
        /// <returns></returns>
        protected async Task AuthenticateAsync()
        {
            var command = new AuthCommand(Password);
            var handler = (OutboundSessionHandler)_channel.Pipeline.Last();
            var reply = await handler.SendCommandAsync(command,
                _channel);
            Authenticated = reply.IsOk;
        }

        /// <summary>
        /// Initialize the tcp channel
        /// </summary>
        protected void Initialize()
        {
            _bootstrap.Group(_eventLoopGroup);
            _bootstrap.Channel<TcpSocketChannel>();
            _bootstrap.Option(ChannelOption.SoLinger,
                1);
            _bootstrap.Option(ChannelOption.TcpNodelay,
                true);
            _bootstrap.Option(ChannelOption.SoKeepalive,
                true);
            _bootstrap.Option(ChannelOption.SoReuseaddr,
                true);
            _bootstrap.Option(ChannelOption.ConnectTimeout,
                ConnectionTimeout);
            _bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                // grab the channel pipeline
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast("FrameDecoder", new FrameDecoder());
                pipeline.AddLast("FrameEncoder", new FrameEncoder());
                pipeline.AddLast("StringEncoder", new StringEncoder());
                pipeline.AddLast("DebugLogging", new LoggingHandler(DotNetty.Handlers.Logging.LogLevel.DEBUG));
                 pipeline.AddLast(new OutboundSessionHandler(this));
            }));
        }
    }
}
