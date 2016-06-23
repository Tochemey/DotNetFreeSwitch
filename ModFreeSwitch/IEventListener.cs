using System.Threading.Tasks;
using ModFreeSwitch.Events;

namespace ModFreeSwitch {
    public interface IEventListener {
        Task OnEventReceived(EslEvent eslEvent);
    }
}