namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Used to connect to FreeSwitch.
    /// </summary>
    public sealed class ConnectCommand : BaseCommand {
        public override string Command => "connect";

        public override string Argument => string.Empty;
    }
}