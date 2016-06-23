namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     GetVar helps retrieve a FreeSwitch Channel Variable using the api command.
    /// </summary>
    public sealed class GetVarCommand : BaseCommand {
        /// <summary>
        ///     The variable name
        /// </summary>
        private readonly string _name;

        /// <summary>
        ///     FreeSwitch Channel Id
        /// </summary>
        private readonly string _uuid;

        public GetVarCommand(string uuid,
            string name) {
            _uuid = uuid;
            _name = name;
        }

        public override string Command {
            get { return "uuid_getvar"; }
        }

        public override string Argument {
            get { return string.Format("{0} {1}", _uuid, _name); }
        }
    }
}