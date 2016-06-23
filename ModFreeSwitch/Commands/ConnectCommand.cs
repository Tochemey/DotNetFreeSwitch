namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Used to connect to FreeSwitch.
    /// </summary>
    public sealed class ConnectCommand : BaseCommand {
        public override string Command {
            get { return "connect"; }
        }

        public override string Argument {
            get { return string.Empty; }
        }
    }
}