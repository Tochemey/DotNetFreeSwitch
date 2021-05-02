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

namespace DotNetFreeSwitch.Common
{
    public enum ChannelState
    {
        /// <summary>
        ///     Channel is created
        /// </summary>
        CS_NEW,

        /// <summary>
        ///     Initialized
        /// </summary>
        CS_INIT,

        /// <summary>
        ///     Going through dial plan
        /// </summary>
        CS_ROUTING,

        /// <summary>
        ///     Passive transmit state
        /// </summary>
        CS_SOFT_EXECUTE,

        /// <summary>
        ///     Executing the dial plan
        /// </summary>
        CS_EXECUTE,

        /// <summary>
        ///     Connected to another channel
        /// </summary>
        CS_EXCHANGE_MEDIA,

        /// <summary>
        ///     Being parked (not same as held)
        /// </summary>
        CS_PARK,

        /// <summary>
        ///     Sending media (as .wav) to channel
        /// </summary>
        CS_CONSUME_MEDIA,

        /// <summary>
        ///     Channel is sleeping
        /// </summary>
        CS_HIBERNATE,

        /// <summary>
        ///     Channel is being reset.
        /// </summary>
        CS_RESET,

        /// <summary>
        ///     Flagged for hangup but not yet terminated.
        /// </summary>
        CS_HANGUP,

        /// <summary>
        ///     Flag is done and ready to be destroyed.
        /// </summary>
        CS_DONE,

        /// <summary>
        ///     Remove the channel
        /// </summary>
        /// &
        CS_DESTROY,

        CS_REPORTING,

        /// <summary>
        ///     Unknown state.
        /// </summary>
        UNKNOWN
    }
}