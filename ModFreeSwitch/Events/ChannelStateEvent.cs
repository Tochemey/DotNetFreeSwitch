using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelStateEvent : EslEvent {
        public ChannelStateEvent(EslMessage message) : base(message) {}

        public string AnswerState => this["Answer-State"];

        public EslChannelDirection CallDirection
            => Enumm.Parse<EslChannelDirection>(this["Call-Direction"]);

        public EslChannelState EslChannelState {
            get {
                var ch = this["Channel-State"];
                return string.IsNullOrEmpty(ch)
                    ? EslChannelState.UNKNOWN
                    : Enumm.Parse<EslChannelState>(ch.Trim());
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

        public EslChannelDirection PresenceCallDirection {
            get {
                var pcd = this["Presence-Call-Direction"];
                return string.IsNullOrEmpty(pcd)
                    ? EslChannelDirection.UNKNOWN
                    : Enumm.Parse<EslChannelDirection>(this["Presence-Call-Direction"]);
            }
        }
    }
}