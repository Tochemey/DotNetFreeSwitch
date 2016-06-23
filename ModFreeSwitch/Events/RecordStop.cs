using System;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class RecordStop : EslEvent {
        public RecordStop(EslMessage message) : base(message) {}
        public string RecordFilePath => this["Record-File-Path"];

        public int RecordMilliSeconds
            =>
                string.IsNullOrEmpty(this["record_ms"])
                    ? 0
                    : this["record_ms"].IsNumeric()
                        ? Convert.ToInt32(this["record_ms"])
                        : 0;

        public int RecordSeconds
            =>
                string.IsNullOrEmpty(this["record_seconds"])
                    ? 0
                    : this["record_seconds"].IsNumeric()
                        ? Convert.ToInt32(this["record_seconds"])
                        : 0;
    }
}