using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    /// <summary>
    ///     Function have been executed on channel
    /// </summary>
    public class ChannelExecute : EslEvent {
        public ChannelExecute(EslMessage message) : base(message) {}

        /// <summary>
        ///     Gets application to execute.
        /// </summary>
        public string Application => this["Application"];

        /// <summary>
        ///     Gets arguments for the application
        /// </summary>
        public string ApplicationData => this["Application-Data"];

        /// <summary>
        ///     Gets response from the application
        /// </summary>
        protected string ApplicationResponse => this["Application-Response"];

        public override string ToString() {
            return "ChannelExecute(" + Application + ", '" + ApplicationData + "')";
        }
    }
}