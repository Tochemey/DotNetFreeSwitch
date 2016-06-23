using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelPark : EslEvent {
        public ChannelPark(EslMessage message) : base(message) {}
    }
}