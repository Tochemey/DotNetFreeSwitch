using System.Collections.Generic;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class Custom : EslEvent {
        public Custom(EslMessage message) : base(message) {}

    }
}