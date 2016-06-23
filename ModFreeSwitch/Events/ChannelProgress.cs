using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    /// <summary>
    ///     A channel have started ringing
    /// </summary>
    public class ChannelProgress : ChannelStateEvent {
        public ChannelProgress(EslMessage message) : base(message) {}
    }
}