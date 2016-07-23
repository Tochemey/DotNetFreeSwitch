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
