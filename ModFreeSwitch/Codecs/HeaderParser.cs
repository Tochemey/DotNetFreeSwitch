namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     Parse freeSwitch headers
    /// </summary>
    public static class HeaderParser {
        /// <summary>
        ///     Split a header in the form
        ///     Header-HeaderName: header-value into a String array.
        /// </summary>
        /// <param name="sb">the string header to parse</param>
        /// <returns>a string[] array with header name at 0 and header value at 1</returns>
        public static string[] SplitHeader(string sb) {
            var len = sb.Length;
            int nameEnd;
            int colonEnd;

            var nameStart = FindNonWhitespace(sb, 0);
            for (nameEnd = nameStart; nameEnd < len; nameEnd++) {
                var ch = sb[nameEnd];
                if (ch == ':' || char.IsWhiteSpace(ch)) break;
            }

            for (colonEnd = nameEnd; colonEnd < len; colonEnd++) {
                if (sb[colonEnd] == ':') {
                    colonEnd++;
                    break;
                }
            }

            var valueStart = FindNonWhitespace(sb, colonEnd);
            if (valueStart == len)
                return new[] {
                    sb.Substring(nameStart, nameEnd),
                    ""
                };

            var valueEnd = FindEndOfString(sb);
            return new[] {
                sb.Substring(nameStart, nameEnd),
                sb.Substring(valueStart, valueEnd)
            };
        }

        private static int FindNonWhitespace(string sb,
            int offset) {
            int result;
            for (result = offset; result < sb.Length; result++)
                if (!char.IsWhiteSpace(sb[result])) break;
            return result;
        }

        private static int FindEndOfString(string sb) {
            int result;
            for (result = sb.Length; result > 0; result--)
                if (!char.IsWhiteSpace(sb[result - 1])) break;
            return result;
        }
    }
}