namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     The auth command helps to authenticate against FreeSwitch Event Socket module.
    /// </summary>
    public sealed class AuthCommand : BaseCommand {
        /// <summary>
        ///     The authentication password
        /// </summary>
        private readonly string _password;

        public AuthCommand(string password) { _password = password; }

        /// <summary>
        ///     Auth Command
        /// </summary>
        public override string Command { get { return "auth"; } }

        /// <summary>
        ///     Auth command argument
        /// </summary>
        public override string Argument { get { return _password; } }
    }
}