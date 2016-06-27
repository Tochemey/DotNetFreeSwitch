using System;
using System.Net;
using System.Threading.Tasks;
using ModFreeSwitch.Events;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch {
    public interface IListener {
        /// <summary>
        ///     This event is raised when a freeSwitch event is received on the channel
        /// </summary>
        /// <param name="eslMessage"></param>
        /// <returns></returns>
        Task OnEventReceived(EslMessage eslMessage);

        /// <summary>
        ///     This event is raised when disconnect/notice message is received on the channel
        /// </summary>
        /// <returns></returns>
        Task OnDisconnectNotice(EslMessage message, EndPoint channelEndPoint);

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

    }
}