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

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     Parse freeSwitch headers
    /// </summary>
    public static class EslHeaderParser {
        /// <summary>
        ///     Split a header in the form
        ///     Header-HeaderName: header-value into a String array.
        /// </summary>
        /// <returns>a string[] array with header name at 0 and header value at 1</returns>
        /// <summary>
        ///     Split a header in the form
        ///     Header-HeaderName: header-value into a String array.
        /// </summary>
        /// <param name="sb">
        ///     the string header to parse
        /// </param>
        /// <returns>a string[] array with header name at 0 and header value at 1</returns>
        public static string[] SplitHeader(string sb) {
            var len = sb.Length;
            int nameEnd;
            int colonEnd;

            var nameStart = FindNonWhitespace(sb, 0);
            for (nameEnd = nameStart; nameEnd < len; nameEnd++) {
                var ch = sb[nameEnd];
                if (ch == ':' || IsWhiteSpace(ch)) break;
            }

            for (colonEnd = nameEnd; colonEnd < len; colonEnd++) {
                if (sb[colonEnd] != ':') continue;
                colonEnd++;
                break;
            }

            var valueStart = FindNonWhitespace(sb, colonEnd);
            return valueStart == len
                ? new[] {
                    sb.Substring(nameStart, nameEnd)
                        .Trim(),
                    ""
                }
                : new[] {
                    sb.Substring(nameStart, nameEnd)
                        .Trim(),
                    sb.Substring(valueStart)
                        .Trim()
                };
        }

        private static int FindNonWhitespace(string sb,
            int offset) {
            int result;
            for (result = offset; result < sb.Length; result++)
                if (!IsWhiteSpace(sb[result])) break;
            return result;
        }

        private static bool IsWhiteSpace(char ch) {
            return ch == '\t' || ch == ' ';
        }
    }
}
