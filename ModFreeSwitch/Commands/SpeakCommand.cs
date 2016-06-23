namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Speak command
    ///     Speaks a string or file of text to the channel using the defined speech engine.
    /// </summary>
    public sealed class SpeakCommand : BaseCommand {
        public SpeakCommand() {
            Engine = "flite";
            Voice = "kal";
        }

        /// <summary>
        ///     Engine
        /// </summary>
        public string Engine { set; get; }

        /// <summary>
        ///     Voice
        /// </summary>
        public string Voice { set; get; }

        /// <summary>
        ///     The actual to read
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        ///     Timer
        /// </summary>
        public string TimerName { set; get; }

        public override string Command {
            get { return "speak"; }
        }

        public override string Argument {
            get {
                return Engine + "|" + Voice + "|" + Text +
                       (!string.IsNullOrEmpty(TimerName) ? "|" + TimerName : "");
            }
        }
    }
}