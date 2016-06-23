using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelBridge : EslEvent {
        public ChannelBridge(EslMessage message) : base(message) {}
    }
}