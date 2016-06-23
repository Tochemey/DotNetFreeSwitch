using System.Linq;

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     Parse freeSwitch headers
    /// </summary>
    public static class EslHeaderParser {
        /// <summary>
        ///     Split a header in the form
        ///     Header-HeaderName: header-value into a String array.
        /// </summary>
        /// <param name="sb">the string header to parse</param>
        /// <returns>a string[] array with header name at 0 and header value at 1</returns>
        public static string[] SplitHeader(string sb) {
            if (string.IsNullOrEmpty(sb)) return null;

            var header = sb.TrimEnd('\n');
            return header.Split(':')
                .Select(c => c.Trim('\n')
                    .Trim())
                .ToArray();
        }
    }
}