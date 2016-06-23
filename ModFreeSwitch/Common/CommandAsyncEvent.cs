using System.Threading.Tasks;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class CommandAsyncEvent {
        private readonly TaskCompletionSource<object> _source;

        public CommandAsyncEvent(BaseCommand command) {
            Command = command;
            _source = new TaskCompletionSource<object>();
        }

        /// <summary>
        ///     The FreeSwitch command to  send
        /// </summary>
        public BaseCommand Command { get; }

        /// <summary>
        ///     The response
        /// </summary>
        public Task<object> Task {
            get { return _source.Task; }
        }

        public void Complete(object response) {
            _source.TrySetResult(response);
        }
    }
}