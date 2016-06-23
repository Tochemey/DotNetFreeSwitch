using System;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Helps to listen a specific channel events.
    /// </summary>
    public sealed class MyEventsCommand : BaseCommand {
        /// <summary>
        ///     Channel Id
        /// </summary>
        private readonly Guid _uuid;

        public MyEventsCommand(Guid uuid) {
            _uuid = uuid;
        }

        public override string Command {
            get { return "myevents"; }
        }

        public override string Argument {
            get { return _uuid.ToString(); }
        }
    }
}