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

namespace Core.Messages
{
    /// <summary>
    ///     FreeSwitch Message Headers
    /// </summary>
    public static class Headers
    {
        /// <summary>
        ///     FreeSwitch message Content-Type header
        /// </summary>
        public const string ContentType = "Content-Type";

        /// <summary>
        ///     FreeSwitch message Content-Length header
        /// </summary>
        public const string ContentLength = "Content-Length";

        /// <summary>
        ///     FreeSwitch message Reply-Text header
        /// </summary>
        public const string ReplyText = "Reply-Text";

        /// <summary>
        ///     FreeSwitch message Job-UUID header
        /// </summary>
        public const string JobUuid = "Job-UUID";

        /// <summary>
        ///     FreeSwitch message Socket-Mode header
        /// </summary>
        public static string SocketMode = "Socket-Mode";

        /// <summary>
        ///     FreeSwitch message Control header
        /// </summary>
        public static string Control = "Control";
    }
}