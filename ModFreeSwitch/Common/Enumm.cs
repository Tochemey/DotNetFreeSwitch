using System;

namespace ModFreeSwitch.Common {
    public static class Enumm {
        public static T Parse<T>(string name) where T : struct {
            T t;
            Enum.TryParse(name, true, out t);
            return t;

            //return (T) Enum.Parse(typeof (T), name, true);
        }
    }
}