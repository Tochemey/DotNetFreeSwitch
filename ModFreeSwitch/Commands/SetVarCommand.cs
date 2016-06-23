namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     SetVar helps set a variable for a specific FreeSwitch channel using the ApiCommand
    /// </summary>
    public sealed class SetVarCommand : BaseCommand {
        /// <summary>
        ///     Variable name
        /// </summary>
        private readonly string _name;

        /// <summary>
        ///     Channel Id
        /// </summary>
        private readonly string _uuid;

        /// <summary>
        ///     Variable value
        /// </summary>
        private readonly string _value;

        public SetVarCommand(string uuid,
            string name,
            string value) {
            _uuid = uuid;
            _name = name;
            _value = value;
        }

        public override string Command {
            get { return "uuid_setvar"; }
        }

        public override string Argument {
            get { return string.Format("{0} {1} {2}", _uuid, Name, Value); }
        }

        /// <summary>
        ///     Variable name
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        ///     Variable value
        /// </summary>
        public string Value {
            get { return _value; }
        }
    }
}