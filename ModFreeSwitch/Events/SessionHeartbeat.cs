using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class SessionHeartbeat : EslEvent {
        public SessionHeartbeat(EslMessage message) : base(message) {}
    }
}