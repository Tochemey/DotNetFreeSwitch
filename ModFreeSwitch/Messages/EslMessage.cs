using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ModFreeSwitch.Codecs;
using NLog;

namespace ModFreeSwitch.Messages {
    /// <summary>
    ///     FreeSwitch decoded message.
    /// </summary>
    public class EslMessage {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public EslMessage() {
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
        public bool HasHeader(string header) {
            return Headers.ContainsKey(header);
        }

        /// <summary>
        ///     Helps retrieve a given header value
        /// </summary>
        /// <param name="header">the header</param>
        /// <returns>string the header value</returns>
        public string HeaderValue(string header) {
            return Headers[header];
        }

        /// <summary>
        ///     Checks whether the freeSwitch message has a content length or not.
        /// </summary>
        /// <returns>true or false</returns>
        public bool HasContentLength() {
            return Headers.ContainsKey(EslHeaders.ContentLength);
        }

        /// <summary>
        ///     Return the message content length when the message has content length or 0 in case of none.
        /// </summary>
        /// <returns>integer the content length</returns>
        public int ContentLength() {
            if (!HasContentLength()) return 0;
            var len = Headers[EslHeaders.ContentLength];
            int contentLength;
            return int.TryParse(len, out contentLength) ? contentLength : 0;
        }

        /// <summary>
        ///     Returns the message content type
        /// </summary>
        /// <returns>string the content type</returns>
        public string ContentType() {
            return Headers[EslHeaders.ContentType];
        }

        /// <summary>
        ///     Checks whether the message we are decoding has a third part. This is very useful for BackgroundJob event
        /// </summary>
        /// <returns></returns>
        public bool HasThirdPart() {
            return BodyLines.Select(EslHeaderParser.SplitHeader)
                .Any(bodyParts => bodyParts[0].Equals(EslHeaders.ContentLength));
        }

        /// <summary>
        ///     Returns the third part content length
        /// </summary>
        /// <returns></returns>
        public int ThirdPartContentLength() {
            foreach (var bodyParts in BodyLines.Select(EslHeaderParser.SplitHeader)
                .Where(bodyParts => bodyParts[0].Equals(EslHeaders.ContentLength))) {
                int len;

                return int.TryParse(bodyParts[1], out len) ? len : 0;
            }
            return 0;
        }

        public override string ToString() {
            var sb = new StringBuilder("EslMessage: contentType=[");
            sb.Append(ContentType());
            sb.Append("] headers=");
            sb.Append(Headers.Count);
            sb.Append(", body=");
            sb.Append(BodyLines.Count);
            sb.Append(" lines.");

            return sb.ToString();
        }
    }
}