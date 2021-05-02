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

namespace DotNetFreeSwitch.Messages
{
    /// <summary>
    ///     FreeSwitch message header values
    /// </summary>
    public static class HeadersValues
    {
        /// <summary>
        ///     Often informs about the status of a command response
        /// </summary>
        internal const string Ok = "+OK";

        /// <summary>
        ///     Only received when connecting to FreeSwitch for the first time. This header values helps track the authentication
        ///     request made by freeSwitch to allow smooth connection to the mod_event_socket.
        /// </summary>
        internal const string AuthRequest = "auth/request";

        /// <summary>
        ///     Qualify an api command response
        /// </summary>
        internal const string ApiResponse = "api/response";

        /// <summary>
        ///     General command response. It is used whenever a command is sent to FreeSwitch
        /// </summary>
        internal const string CommandReply = "command/reply";

        /// <summary>
        ///     This header value helps track freeSwitch events
        /// </summary>
        internal const string TextEventPlain = "text/event-plain";

        /// <summary>
        ///     Same as <see cref="TextEventPlain" />. However the freeSwitch events messages are sent in XML format.
        /// </summary>
        public const string TextEventXml = "text/event-xml";

        /// <summary>
        ///     Qualifies a disconnection from freeSwitch mod_event_socket
        /// </summary>
        internal const string TextDisconnectNotice = "text/disconnect-notice";

        /// <summary>
        ///     In conjunction with <see cref="Ok" /> it helps to know the status of any command sent to freeSwitch via
        ///     mod_event_socket as an invalid command.
        /// </summary>
        public const string ErrInvalid = "-ERR invalid";

        /// <summary>
        ///     <see cref="ErrInvalid" />. Here the command sent results in errors.
        /// </summary>
        public const string Err = "-ERR";

        /// <summary>
        ///     Often observed during connection to freeSwitch mod_event_socket. It often happens when there is network issue.
        /// </summary>
        internal const string TextRudeRejection = "text/rude-rejection";
    }
}