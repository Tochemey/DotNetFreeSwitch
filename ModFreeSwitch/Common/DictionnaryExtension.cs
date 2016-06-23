using System.Collections.Generic;
using System.Linq;

namespace ModFreeSwitch.Common {
    public static class DictionnaryExtension {
        public static string ToStringFlattened(this Dictionary<string, string> source,
            string keyValueSeparator = "=",
            string sequenceSeparator = "|") {
            return source == null
                ? ""
                : string.Join(sequenceSeparator,
                    source.Keys.Zip(source.Values,
                        (k,
                            v) => k + keyValueSeparator + v));
        }
    }
}