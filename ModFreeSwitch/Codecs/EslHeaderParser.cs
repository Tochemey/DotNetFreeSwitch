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
        //public static string[] SplitHeader(string sb) {
        //    if (string.IsNullOrEmpty(sb)) return null;

        //    var header = sb.TrimEnd('\n');
        //    return header.Split(':')
        //        .Select(c => c.Trim('\n')
        //            .Trim())
        //        .ToArray();
        //}

        /// <summary>
        ///     Split a header in the form
        ///     Header-HeaderName: header-value into a String array.
        /// </summary>
        /// <param name="sb">the string header to parse</param>
        /// <returns>a string[] array with header name at 0 and header value at 1</returns>
        public static string[] SplitHeader(string sb)
        {
            int len = sb.Length;
            int nameEnd;
            int colonEnd;

            var nameStart = FindNonWhitespace(sb, 0);
            for (nameEnd = nameStart; nameEnd < len; nameEnd++)
            {
                char ch = sb[nameEnd];
                if (ch == ':'
                    || IsWhiteSpace(ch)) break;
            }

            for (colonEnd = nameEnd; colonEnd < len; colonEnd++)
            {
                if (sb[colonEnd] == ':')
                {
                    colonEnd++;
                    break;
                }
            }

            var valueStart = FindNonWhitespace(sb, colonEnd);
            if (valueStart == len) return new[] { sb.Substring(nameStart, nameEnd).Trim(), "" };

            //var valueEnd = FindEndOfString(sb);
            return new[] { sb.Substring(nameStart, nameEnd).Trim(), sb.Substring(valueStart).Trim() };
        }

        private static int FindNonWhitespace(string sb, int offset)
        {
            int result;
            for (result = offset; result < sb.Length; result++) if (!IsWhiteSpace(sb[result])) break;
            return result;
        }

        private static int FindEndOfString(string sb)
        {
            int result;
            for (result = sb.Length; result > 0; result--) if (!IsWhiteSpace(sb[result - 1])) break;
            return result;
        }

        private static bool IsEOL(char ch)
        {
            return ch == '\r' || ch == '\n';
        }


        private static bool IsWhiteSpace(char ch)
        {
            return ch == '\t' || ch == ' ';
        }

    }
}