using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelUnbridge : EslEvent {
        public ChannelUnbridge(EslMessage message) : base(message) {}
    }
}