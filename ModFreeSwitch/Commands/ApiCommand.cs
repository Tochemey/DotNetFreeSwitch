namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     FreeSwitch API command.
    /// </summary>
    public sealed class ApiCommand : BaseCommand {
        /// <summary>
        ///     The command string to send
        /// </summary>
        private readonly string _apiCommand;

        public ApiCommand(string apiCommand) {
            _apiCommand = apiCommand;
        }

        /// <summary>
        ///     The api command itself
        /// </summary>
        public override string Command {
            get { return "api"; }
        }

        /// <summary>
        ///     The api command argument
        /// </summary>
        public override string Argument {
            get { return _apiCommand; }
        }
    }
}