using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Handlers.inbound {
    public interface IInboundListener : IListener {

        /// <summary>
        /// Helps gather the connected call data
        /// </summary>
        /// <param name="connectedInfo"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task OnConnected(ConnectedCall connectedInfo, IChannel channel);
    }
}