using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using NLog;

namespace ModFreeSwitch.Messages {
    /// <summary>
    ///     FreeSwitch decoded message.
    /// </summary>
    public class EventSocketMessage {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     FreeSwitch decoded message headers.
        /// </summary>
        public StringDictionary Headers { set; get; }

        /// <summary>
        ///     FreeSwitch decoded message body lines.
        /// </summary>
        public List<string> BodyLines { set; get; }

        /// <summary>
        ///     Checks whether the freeSwitch message has a given header.
        /// </summary>
        /// <param name="header">the header</param>
        /// <returns>true or false</returns>
        public bool HasHeader(string header) { return Headers.ContainsKey(header); }

        /// <summary>
        ///     Helps retrieve a given header value
        /// </summary>
        /// <param name="header">the header</param>
        /// <returns>string the header value</returns>
        public string HeaderValue(string header) { return Headers[header]; }

        /// <summary>
        ///     Checks whether the freeSwitch message has a content length or not.
        /// </summary>
        /// <returns>true or false</returns>
        public bool HasContentLength() { return Headers.ContainsKey(EventSocketHeaders.ContentLength); }

        /// <summary>
        ///     Return the message content length when the message has content length or 0 in case of none.
        /// </summary>
        /// <returns>integer the content length</returns>
        public int ContentLength() {
            if (!HasContentLength()) return 0;
            var len = Headers[EventSocketHeaders.ContentLength];
            int contentLength;
            return int.TryParse(len, out contentLength) ? contentLength : 0;
        }

        /// <summary>
        ///     Returns the message content type
        /// </summary>
        /// <returns>string the content type</returns>
        public string ContentType() { return Headers[EventSocketHeaders.ContentType]; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder("FreeSwitchMessage: contentType=[");
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