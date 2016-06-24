using System;
using System.Threading.Tasks;
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Handlers.outbound {
    public interface IOutboundListener : IListener{

        /// <summary>
        /// Raised when authentication request occurs
        /// </summary>
        /// <returns></returns>
        Task OnAuthentication();
    }
}