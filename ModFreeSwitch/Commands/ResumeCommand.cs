namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Resume command
    /// </summary>
    public sealed class ResumeCommand : BaseCommand {
        public override string Command {
            get { return "resume"; }
        }

        public override string Argument {
            get { return string.Empty; }
        }
    }
}