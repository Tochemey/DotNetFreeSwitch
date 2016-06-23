namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Helps Schedule some command to be executed.
    /// </summary>
    public sealed class SchedApiCommand : BaseCommand {
        private readonly bool _asynchronous;
        private readonly string _command;
        private readonly string _groupName;
        private readonly bool _repetitive;
        private readonly int _time;

        public SchedApiCommand(string command,
            string groupName,
            bool repetitive,
            int time,
            bool asynchronous) {
            _command = command;
            _groupName = groupName;
            _repetitive = repetitive;
            _time = time;
            _asynchronous = asynchronous;
        }

        public override string Command {
            get { return "sched_api"; }
        }

        public override string Argument {
            get {
                var args = string.Format("+{0} {1} {2} {3}",
                    _time,
                    _groupName,
                    _command,
                    _asynchronous ? "&" : string.Empty);
                if (_repetitive)
                    args = string.Format("@{0} {1} {2} {3}",
                        _time,
                        _groupName,
                        _command,
                        _asynchronous ? "&" : string.Empty);
                return args;
            }
        }
    }
}