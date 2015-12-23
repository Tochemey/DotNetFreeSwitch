namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     play and get digits
    /// </summary>
    public sealed class PlayAndGetDigitsCommand : BaseCommand {
        public PlayAndGetDigitsCommand() {
            MaxNumberOfDigits = 128;
            MinNumberOfDigits = 0;
            Terminators = '#';
            Retries = 1;
            Regex = "1234567890*#";
            DigitTimeout = (2*1000);
            Timeout = (5*1000);
            InvalidFile = "silence_stream://150";
        }

        public override string Argument
        {
            get
            {
                var argv = string.Format("{0} {1} {2} {3} '{4}' '{5}' {6} {7} {8} {9}", MinNumberOfDigits, MaxNumberOfDigits, Retries, Timeout, Terminators, SoundFile, InvalidFile, VariableName, Regex, DigitTimeout);
                return argv;
            }
        }

        public override string Command { get { return "play_and_get_digits"; } }

        /// <summary>
        ///     Inter-digit timeout; number of milliseconds allowed between digits; once this number is reached, PAGD assumes that
        ///     the caller has no more digits to dial
        /// </summary>
        public int DigitTimeout { set; get; }

        /// <summary>
        ///     Sound file to play when digits don't match the regexp
        /// </summary>
        public string InvalidFile { set; get; }

        /// <summary>
        ///     Maximum number of digits to fetch (maximum value of 128)
        /// </summary>
        public int MaxNumberOfDigits { set; get; }

        /// <summary>
        ///     Minimum number of digits to fetch (minimum value of 0)
        /// </summary>
        public int MinNumberOfDigits { set; get; }

        /// <summary>
        ///     Plays a beep when each digit entered.
        /// </summary>
        public bool PlayBeep { set; get; }

        /// <summary>
        ///     Regular expression to match digits
        /// </summary>
        public string Regex { set; get; }

        /// <summary>
        ///     numbers of tries for the sound to play
        /// </summary>
        public int Retries { set; get; }

        /// <summary>
        ///     Sound file to play while digits are fetched
        /// </summary>
        public string SoundFile { set; get; }

        /// <summary>
        ///     digits used to end input if less than <see cref="MaxNumberOfDigits" /> digits have been pressed. (Typically '#')
        /// </summary>
        public char Terminators { set; get; }

        /// <summary>
        ///     Number of milliseconds to wait for a dialed response after the file playback ends and before PAGD does a retry.
        /// </summary>
        public int Timeout { set; get; }

        /// <summary>
        ///     Channel variable into which digits should be placed
        /// </summary>
        public string VariableName { set; get; }
    }
}