using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Handlers.outbound {
    /// <summary>
    ///     EslClientHandler. This class will handle all request and responses that will go to freeSwitch.
    /// </summary>
    public class EslClientHandler : ChannelHandlerAdapter {
        private bool _authenticated;

        private readonly Queue<CommandAsyncEvent> _commandAsyncEvents; 
        /// <summary>
        ///     This password is used to connect to mod_event_socket module of freeSwitch.
        /// </summary>
        private readonly string _password;

        public EslClientHandler(string password) {
            _password = password;
            _commandAsyncEvents = new Queue<CommandAsyncEvent>();
        }

        public EslClientHandler() : this("ClueCon") {}

        /// <summary>
        ///     This helps read the data received from the socket client.
        /// </summary>
        /// <param name="context">Channel context</param>
        /// <param name="message">the decoded message received</param>
        public override async void ChannelRead(IChannelHandlerContext context,
            object message) {
            var decodedMessage = message as EslMessage;
            if (decodedMessage != null) {
                var contentType = decodedMessage.ContentType();
                if (contentType.Equals(EslHeadersValues.AuthRequest)) {
                    await Authenticate(context);
                    return;
                }

                if (contentType.Equals(EslHeadersValues.CommandReply)) {
                    CommandAsyncEvent  commandAsyncEvent = _commandAsyncEvents.Dequeue();
                    CommandReply reply = new CommandReply(commandAsyncEvent.Command.Command, decodedMessage);
                    commandAsyncEvent.Complete(reply);
                }
            }
        }

        public async Task<CommandReply> SendCommand(BaseCommand command, IChannelHandlerContext context) {
            CommandAsyncEvent asyncEvent = new CommandAsyncEvent(command);
            _commandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task;
        }

        protected async Task Authenticate(IChannelHandlerContext context) {
            AuthCommand command = new AuthCommand(_password);
            CommandReply reply = await SendCommand(command, context);
            _authenticated = reply.IsOk;
        }

        protected bool IsReady() {
            return _authenticated;
        }
    }
}