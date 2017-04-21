using ModFreeSwitch.Events;

namespace ModFreeSwitch.Common
{
    public class EslEventStream
    {
        public EslEventStream(EslEvent eslEvent,
            EslEventType eslEventType)
        {
            EslEvent = eslEvent;
            EslEventType = eslEventType;
        }

        public EslEvent EslEvent { get; }
        public EslEventType EslEventType { get; }
    }
}