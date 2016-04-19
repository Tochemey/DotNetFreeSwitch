

namespace ModFreeSwitch.Common {
    /// <summary>
    ///     Represent a FreeSwitch channel variable
    /// </summary>
    public sealed class EslChannelVariable {
        public EslChannelVariable(string name, string value) {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }

        public override string ToString() { return Name + "=" + Value; }
    }
}