using System;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Handlers.outbound {
    /// <summary>
    ///     Event Socket Client handler
    /// </summary>
    public abstract class EventSocketClientHandler : ChannelHandlerAdapter {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Read data received from the freeSwitch mod_event_socket
        /// </summary>
        /// <param name="context">the channel context</param>
        /// <param name="message">the data received from socket</param>
        public override void ChannelRead(IChannelHandlerContext context, object message) {
            // We know that on good note the message object will be the EventSocketMessage after decoding.
            EventSocketMessage msg = message as EventSocketMessage;

            if (logger.IsDebugEnabled) logger.Debug("Message received [{0}]", msg);

            string contentType = msg?.ContentType();
            if (string.IsNullOrEmpty(contentType)) return;
            if (contentType.Equals(EventSocketHeadersValues.TextEventPlain)) {
                EventReceived?.Invoke(this, new EventSocketArgs(context, msg));
                return;
            }

            if (contentType.Equals(EventSocketHeadersValues.ApiResponse)) {
                ApiResponse?.Invoke(this, new EventSocketArgs(context, msg));
                return;
            }

            if (contentType.Equals(EventSocketHeadersValues.CommandReply)) {
                CommandReply?.Invoke(this, new EventSocketArgs(context, msg));
                return;
            }

            if (contentType.Equals(EventSocketHeadersValues.AuthRequest)) {
                AuthRequest?.Invoke(this, new EventSocketArgs(context, msg));
                return;
            }

            if (contentType.Equals(EventSocketHeadersValues.TextDisconnectNotice)) {
                DisconnectNotice?.Invoke(this, new EventSocketArgs(context, msg));
                return;
            }

            if (!contentType.Equals(EventSocketHeadersValues.TextRudeRejection)) return;
            RudeRejection?.Invoke(this, new EventSocketArgs(context, msg));
        }

        /// <summary>
        ///     Executed when the <see cref="ChannelRead" /> has finished its work
        /// </summary>
        /// <param name="context">the channel context</param>
        public override void ChannelReadComplete(IChannelHandlerContext context) { context.Flush(); }

        /// <summary>
        ///     This event is fired when an auth/request message is receiced. It  is very useful to complete connection to
        ///     freeSwitch
        ///     mod_event_socket
        /// </summary>
        protected event EventHandler<EventSocketArgs> AuthRequest = delegate { };

        /// <summary>
        ///     This event is fired whenever an event is received from freeSwitch mod_event_socket
        /// </summary>
        protected event EventHandler<EventSocketArgs> EventReceived = delegate { };

        /// <summary>
        ///     This event is fired when a disconnect notice is received.
        /// </summary>
        protected event EventHandler<EventSocketArgs> DisconnectNotice = delegate { };

        /// <summary>
        ///     Event fired when a rude rejection is received.
        /// </summary>
        protected event EventHandler<EventSocketArgs> RudeRejection = delegate { };

        /// <summary>
        ///     Fired whenever a command/reply is received
        /// </summary>
        protected event EventHandler<EventSocketArgs> CommandReply = delegate { };

        /// <summary>
        ///     Fired whenever a api/response is received
        /// </summary>
        protected event EventHandler<EventSocketArgs> ApiResponse = delegate { };
    }
}