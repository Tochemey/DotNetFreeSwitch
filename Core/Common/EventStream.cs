using Core.Events;

namespace Core.Common
{
    public class EventStream
    {
        public EventStream(FsEvent fsEvent,
            EventType eventType)
        {
            FsEvent = fsEvent;
            EventType = eventType;
        }

        public FsEvent FsEvent { get; }
        public EventType EventType { get; }
    }
}