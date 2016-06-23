using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelHangup : EslEvent {
        public ChannelHangup(EslMessage message) : base(message) {}

        public HangupCause Cause => Enumm.Parse<HangupCause>(this["Hangup-Cause"]);
    }
}