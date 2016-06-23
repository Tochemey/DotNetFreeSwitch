namespace ModFreeSwitch.Commands {
    public sealed class ExitCommand : BaseCommand {
        public override string Command {
            get { return "exit"; }
        }

        public override string Argument {
            get { return string.Empty; }
        }
    }
}