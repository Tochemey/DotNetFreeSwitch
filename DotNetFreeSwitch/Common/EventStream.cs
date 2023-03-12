using DotNetFreeSwitch.Events;

namespace DotNetFreeSwitch.Common
{
   public class EventStream
   {
      public EventStream(Event fsEvent,
          EventType eventType)
      {
         FsEvent = fsEvent;
         EventType = eventType;
      }

      public Event FsEvent { get; }
      public EventType EventType { get; }
   }
}