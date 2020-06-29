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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Codecs;
using NLog;

namespace Core.Messages
{
    /// <summary>
    ///     FreeSwitch decoded message.
    /// </summary>
    public class FsMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FsMessage()
        {
            Headers = new Dictionary<string, string>();
            BodyLines = new List<string>();
        }

        /// <summary>
        ///     FreeSwitch decoded message headers.
        /// </summary>
        public Dictionary<string, string> Headers { set; get; }

        /// <summary>
        ///     FreeSwitch decoded message body lines.
        /// </summary>
        public List<string> BodyLines { set; get; }

        /// <summary>
        ///     Checks whether the freeSwitch message has a given header.
        /// </summary>
        /// <param name="header">the header</param>
        /// <returns>true or false</returns>
        public bool HasHeader(string header)
        {
            return Headers.ContainsKey(header);
        }

        /// <summary>
        ///     Helps retrieve a given header value
        /// </summary>
        /// <param name="header">the header</param>
        /// <returns>string the header value</returns>
        public string HeaderValue(string header)
        {
            return Headers[header];
        }

        /// <summary>
        ///     Checks whether the freeSwitch message has a content length or not.
        /// </summary>
        /// <returns>true or false</returns>
        public bool HasContentLength()
        {
            return Headers.ContainsKey(Messages.Headers.ContentLength);
        }

        /// <summary>
        ///     Return the message content length when the message has content length or 0 in case of none.
        /// </summary>
        /// <returns>integer the content length</returns>
        public int ContentLength()
        {
            if (!HasContentLength()) return 0;
            var len = Headers[Messages.Headers.ContentLength];
            return int.TryParse(len,
                out var contentLength)
                ? contentLength
                : 0;
        }

        /// <summary>
        ///     Returns the message content type
        /// </summary>
        /// <returns>string the content type</returns>
        public string ContentType()
        {
            return HasHeader(Messages.Headers.ContentType) ? Headers[Messages.Headers.ContentType] : string.Empty;
        }

        /// <summary>
        ///     Checks whether the message we are decoding has a third part. This is very useful for BackgroundJob event
        /// </summary>
        /// <returns></returns>
        public bool HasThirdPart()
        {
            return BodyLines.Select(EslHeaderParser.SplitHeader)
                .Any(bodyParts => bodyParts[0].Equals(Messages.Headers.ContentLength));
        }

        /// <summary>
        ///     Returns the third part content length
        /// </summary>
        /// <returns></returns>
        public int ThirdPartContentLength()
        {
            foreach (var bodyParts in BodyLines.Select(EslHeaderParser.SplitHeader)
                .Where(bodyParts => bodyParts[0].Equals(Messages.Headers.ContentLength)))
                return int.TryParse(bodyParts[1],
                    out var len)
                    ? len
                    : 0;
            return 0;
        }

        public Dictionary<string, string> ParseBodyLines()
        {
            var resp = new Dictionary<string, string>();
            foreach (var bodyLine in BodyLines)
            {
                var parsedLines = EslHeaderParser.SplitHeader(bodyLine);
                if (parsedLines == null) continue;
                switch (parsedLines.Length)
                {
                    case 2:
                        resp.Add(parsedLines[0],
                            parsedLines[1]);
                        break;
                    case 1:
                        resp.Add("__CONTENT__",
                            bodyLine);
                        break;
                }
            }

            return resp;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var str in Headers.Keys) sb.AppendLine(str + ":" + Headers[str]);
            sb.AppendLine();

            var map = ParseBodyLines();
            foreach (var str in map.Keys) sb.AppendLine(str + ":" + map[str]);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}