using System.Threading.Tasks;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class CommandAsyncEvent {
        private readonly BaseCommand _command;
        private readonly TaskCompletionSource<CommandReply> _source;

        public CommandAsyncEvent(BaseCommand command) {
            _command = command;
            _source = new TaskCompletionSource<CommandReply>();
        }

        /// <summary>
        ///     The FreeSwitch command to  send
        /// </summary>
        public BaseCommand Command { get { return _command; } }

        /// <summary>
        ///     The response
        /// </summary>
        public Task<CommandReply> Task { get { return _source.Task; } }

        public void Complete(CommandReply response) { _source.TrySetResult(response); }
    }
}