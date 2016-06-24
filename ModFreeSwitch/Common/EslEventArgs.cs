using System;
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Common {
    /// <summary>
    /// </summary>
    public class EslEventArgs : EventArgs {
        public EslEventArgs(EslEvent eslEvent) {
            EslEvent = eslEvent;
        }

        public EslEvent EslEvent { get; private set; }
    }
}