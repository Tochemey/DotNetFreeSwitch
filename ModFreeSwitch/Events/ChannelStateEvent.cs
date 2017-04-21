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

using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events
{
    public class ChannelStateEvent : EslEvent
    {
        public ChannelStateEvent(EslMessage message) : base(message) { }

        public string AnswerState => this["Answer-State"];

        public EslChannelDirection CallDirection => Enumm.Parse<EslChannelDirection>(this["Call-Direction"]);

        public EslChannelState EslChannelState
        {
            get
            {
                var ch = this["Channel-State"];
                return string.IsNullOrEmpty(ch) ? EslChannelState.UNKNOWN : Enumm.Parse<EslChannelState>(ch.Trim());
            }
        }

        public CallState CallState
        {
            get
            {
                var cs = this["Channel-Call-State"];
                return string.IsNullOrEmpty(cs) ? CallState.DOWN : Enumm.Parse<CallState>(cs);
            }
        }

        public EslChannelDirection PresenceCallDirection
        {
            get
            {
                var pcd = this["Presence-Call-Direction"];
                return string.IsNullOrEmpty(pcd) ? EslChannelDirection.UNKNOWN : Enumm.Parse<EslChannelDirection>(this["Presence-Call-Direction"]);
            }
        }
    }
}