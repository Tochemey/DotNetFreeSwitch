using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelStateEvent : EslEvent {
        public ChannelStateEvent(EslMessage message) : base(message) {}

        public string AnswerState => this["Answer-State"];

        public ChannelDirection CallDirection
            => Enumm.Parse<ChannelDirection>(this["Call-Direction"]);

        public ChannelState ChannelState {
            get {
                var ch = this["Channel-State"];
                return string.IsNullOrEmpty(ch)
                    ? ChannelState.UNKNOWN
                    : Enumm.Parse<ChannelState>(ch.Trim());
            }
        }

        public CallState CallState {
            get {
                var cs = this["Channel-Call-State"];
                return string.IsNullOrEmpty(cs)
                    ? CallState.DOWN
                    : Enumm.Parse<CallState>(cs);
            }
        }

        public ChannelDirection PresenceCallDirection {
            get {
                var pcd = this["Presence-Call-Direction"];
                return string.IsNullOrEmpty(pcd)
                    ? ChannelDirection.UNKNOWN
                    : Enumm.Parse<ChannelDirection>(this["Presence-Call-Direction"]);
            }
        }
    }
}