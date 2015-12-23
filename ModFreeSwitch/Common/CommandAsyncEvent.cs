using System.Threading.Tasks;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class CommandAsyncEvent {
        private readonly object _command;
        private readonly TaskCompletionSource<CommandReplyMessage> _source;

        public CommandAsyncEvent(BaseCommand command) {
            _command = command;
            _source = new TaskCompletionSource<CommandReplyMessage>();
        }

        /// <summary>
        ///     The FreeSwitch command to  send
        /// </summary>
        public object Command { get { return _command; } }

        /// <summary>
        ///     The response
        /// </summary>
        public Task<CommandReplyMessage> Task { get { return _source.Task; } }

        public void Complete(CommandReplyMessage response) { _source.TrySetResult(response); }
    }
}