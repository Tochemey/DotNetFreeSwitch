namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     sleep.
    ///     Pause the channel for a given number of milliseconds, consuming the audio for that period of time.
    ///     Calling sleep also will consume any outstanding RTP on the operating system's input queue, which can be very useful
    ///     in situations where audio becomes backlogged.
    /// </summary>
    public sealed class SleepCommand : BaseCommand {
        public SleepCommand(long duration) {
            Duration = duration;
        }

        /// <summary>
        ///     How long to pause the channel
        /// </summary>
        public long Duration { get; }

        public override string Command {
            get { return "sleep"; }
        }

        public override string Argument {
            get { return Duration.ToString(); }
        }
    }
}