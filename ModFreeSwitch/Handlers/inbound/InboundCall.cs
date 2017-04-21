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
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Handlers.inbound
{
    /// <summary>
    ///     Holds the details of the connected call
    /// </summary>
    public class InboundCall
    {
        private readonly EslEvent _event;

        public InboundCall(EslEvent @event) { _event = @event; }

        public Guid CallerGuid => Guid.Parse(_event["Caller-Unique-ID"]);
        public string CallerId => _event["Caller-Caller-ID-Number"];
        public string ChannelName => _event["Channel-Name"];
        public string DestinationNumber => _event["Channel-Destination-Number"];
        public Guid UniqueId => Guid.Parse(_event["Unique-ID"]);
        public string UserContext => _event["user_context"];
        public string this[string name] => _event[name];
    }
}