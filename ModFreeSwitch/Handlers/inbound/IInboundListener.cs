using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Events;

namespace ModFreeSwitch.Handlers.inbound {
    public interface IInboundListener {
        /// <summary>
        ///     This event is raised when a freeSwitch event is received on the channel
        /// </summary>
        /// <param name="eslEvent"></param>
        /// <returns></returns>
        Task OnEventReceived(EslEvent eslEvent);

        /// <summary>
        ///     This event is raised when disconnect/notice message is received on the channel
        /// </summary>
        /// <returns></returns>
        Task OnDisconnectNotice();

        /// <summary>
        /// This event is raised when we received a rude/rejection message on the channel
        /// </summary>
        /// <returns></returns>
        Task OnRudeRejection();

        /// <summary>
        /// This event is raised when an error occurs on the channel
        /// </summary>
        /// <param name="exception"></param>
        Task OnError(Exception exception);

        /// <summary>
        /// Helps gather the connected call data
        /// </summary>
        /// <param name="connectedInfo"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        Task OnConnected(ConnectedCall connectedInfo, IChannel channel);
    }
}