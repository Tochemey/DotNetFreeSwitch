/*
    Copyright [2016] [Arsene Tochemey GANDOTE]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Text;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class EslEvent {
        private readonly bool _ignoreBody;
        private readonly EslMessage _response;

        public EslEvent(EslMessage response,
            bool ignoreBody) {
            _response = response;
            _ignoreBody = ignoreBody;
        }

        public EslEvent(EslMessage response) : this(response, false) {}

        public Guid CallerGuid => Guid.Parse(this["Caller-Unique-ID"]);

        public Guid ChannelCallGuid => Guid.Parse(this["Channel-Call-UUID"]);

        public string ChannelName => this["Channel-Name"];

        public Guid CoreGuid => Guid.Parse(this["Core-UUID"]);

        public string EventCallingFile => this["Event-Calling-File"];

        public string EventCallingFunction => this["Event-Calling-Function"];

        public string EventCallingLineNumber => this["Event-Calling-Line-Number"];

        public DateTime EventDateGmt => DateTime.Parse(this["Event-Date-GMT"]);

        public DateTime EventDateLocal => DateTime.Parse(this["Event-Date-Local"]);

        public string EventName => this["Event-Name"];

        public string EventSubClass => this["Event-Subclass"];

        public DateTime EventTimeStamp => this["Event-Date-timestamp"].FromUnixTime();

        public Guid UniqueId => Guid.Parse(this["Unique-ID"]);

        public string this[string headerName] {
            get {
                if (_ignoreBody) {
                    if (_response.HasHeader(headerName))
                        return _response.HeaderValue(headerName);
                    if (_response.HasHeader("variable_" + headerName))
                        return _response.HeaderValue("variable_" + headerName);
                }

                var map = _response.ParseBodyLines();
                if (map.ContainsKey(headerName)) return map[headerName];
                return map.ContainsKey("variable_" + headerName)
                    ? map["variable_" + headerName]
                    : null;
            }
        }

        //protected Dictionary<string, string> ParseBodyLines() {
        //    var resp = new Dictionary<string, string>();
        //    foreach (var bodyLine in _response.BodyLines) {
        //        var parsedLines = EslHeaderParser.SplitHeader(bodyLine);
        //        if (parsedLines == null) continue;
        //        if (parsedLines.Length == 2) resp.Add(parsedLines[0], parsedLines[1]);
        //        if (parsedLines.Length == 1) resp.Add("__CONTENT__", bodyLine);
        //    }
        //    return resp;
        //}

        public override string ToString() {
            var sb = new StringBuilder();
            if (_ignoreBody) {
                foreach (var str in _response.Headers.Keys)
                    sb.AppendLine(str + ":" + _response.Headers[str]);
                return sb.ToString();
            }

            var map = _response.ParseBodyLines();
            foreach (var str in map.Keys)
                sb.AppendLine(str + ":" + map[str]);
            return sb.ToString();
        }
    }
}
