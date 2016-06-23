using System.Collections.Generic;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    /// <summary>
    ///     A channel has started early media
    /// </summary>
    public class ChannelProgressMedia : EslEvent {
        public ChannelProgressMedia(EslMessage message) : base(message) {}

    }
}