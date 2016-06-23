using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    /// <summary>
    ///     Result for a background job.
    /// </summary>
    public class BackgroundJob : EslEvent {
        public BackgroundJob(EslMessage message) : base(message) {}

        /// <summary>
        ///     Gets ID of the bgapi job
        /// </summary>
        public string JobUid => this["Job-UUID"];

        /// <summary>
        ///     Gets command which was executed
        /// </summary>
        public string CommandName => this["Job-Command"];

        /// <summary>
        ///     Gets arguments for the command
        /// </summary>
        public string CommandArguments => this["Job-Command-Arg"];

        /// <summary>
        ///     Gets the actual command result.
        /// </summary>
        public string CommandResult {
            get {
                var commandResult = this["__CONTENT__"];
                if (!string.IsNullOrEmpty(commandResult)) return commandResult;
                return commandResult;
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return CommandName + "(" + CommandArguments + ") = '" + CommandResult +
                   "'\r\n\t";
        }
    }
}