namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Record command.
    ///     Record is used for recording messages, like in a voicemail system
    /// </summary>
    public sealed class RecordCommand : BaseCommand {
        public RecordCommand() { SilenceHit = 3; }

        public override string Argument { get { return string.Format("{0} {1} {2} {3}", RecordFile, TimeLimit, SilenceTreshold, SilenceHit); } }

        public override string Command { get { return "record"; } }

        /// <summary>
        ///     File to record
        /// </summary>
        public string RecordFile { set; get; }

        /// <summary>
        ///     how many seconds of audio below silence_thresh will be tolerated before the recording stops. When omitted, the
        ///     default value is 3.
        /// </summary>
        public long SilenceHit { set; get; }

        /// <summary>
        ///     the energy level below which is considered silence.
        /// </summary>
        public long SilenceTreshold { set; get; }

        /// <summary>
        ///     the maximum duration of the recording in seconds.
        /// </summary>
        public long TimeLimit { set; get; }
    }
}