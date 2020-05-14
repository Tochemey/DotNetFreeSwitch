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
    ///     Command/Reply message
    /// </summary>
    public sealed class CommandReply
    {
        public CommandReply(string command,
            FsMessage response)
        {
            Command = command;
            Response = response;
            ReplyText = Response != null ? Response.HeaderValue(Headers.ReplyText) : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) && ReplyText.StartsWith(HeadersValues.Ok);
        }

        public string Command { get; }

        public FsMessage Response { get; }

        /// <summary>
        ///     Actual reply text
        /// </summary>
        public string ReplyText { get; }

        /// <summary>
        ///     Check whether the command has been successful or not.
        /// </summary>
        public bool IsOk { get; }

        public string this[string headerName] => Response.HeaderValue(headerName);

        /// <summary>
        ///     Returns the actual command/reply type
        /// </summary>
        public string ContentType => Response.ContentType();
    }
}