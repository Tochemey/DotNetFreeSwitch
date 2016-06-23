using System;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class Dtmf : EslEvent {
        public Dtmf(EslMessage message) : base(message) {}

        public char Digit => Convert.ToChar(this["DTMF-Digit"]);

        public int Duration {
            get {
                int duration;
                return int.TryParse(this["DTMF-Duration"], out duration) ? duration : 0;
            }
        }

        public override string ToString() {
            return "Dtmf(" + Digit + ").";
        }
    }
}