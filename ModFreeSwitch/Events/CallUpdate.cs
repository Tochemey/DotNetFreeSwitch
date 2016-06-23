using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class CallUpdate : EslEvent {
        public CallUpdate(EslMessage message) : base(message) {}
    }
}