namespace ModFreeSwitch.Commands {
    public sealed class EventsCommand : BaseCommand {
        /// <summary>
        ///     Space separated list of events
        /// </summary>
        private readonly string _events;

        public EventsCommand(string events) {
            _events = events;
        }

        public override string Command {
            get { return "events"; }
        }

        public override string Argument {
            get { return _events; }
        }
    }
}