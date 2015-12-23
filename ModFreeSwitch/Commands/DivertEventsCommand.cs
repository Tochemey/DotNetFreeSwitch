namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     DivertEvent
    /// </summary>
    public sealed class DivertEventsCommand : BaseCommand {
        private readonly bool _flag;
        public DivertEventsCommand(bool flag) { _flag = flag; }

        public override string Command { get { return "divert_events"; } }

        public override string Argument { get { return _flag ? "on" : "off"; } }
    }
}