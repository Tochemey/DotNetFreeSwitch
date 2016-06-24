using System;
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Handlers.inbound {
    /// <summary>
    ///     Holds the details of the connected call
    /// </summary>
    public class ConnectedCall {
        private readonly EslEvent _event;

        public ConnectedCall(EslEvent @event) {
            _event = @event;
        }

        public Guid CallerGuid => Guid.Parse(_event["Caller-Unique-ID"]);
        public string CallerId => _event["Caller-Caller-ID-Number"];
        public string ChannelName => _event["Channel-Name"];
        public string DestinationNumber => _event["Channel-Destination-Number"];
        public Guid UniqueId => Guid.Parse(_event["Unique-ID"]);
        public string UserContext => _event["user_context"];
        public string this[string name] => _event[name];
    }
}