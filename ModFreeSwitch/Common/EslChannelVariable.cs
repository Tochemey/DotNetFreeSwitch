namespace ModFreeSwitch.Common {
    /// <summary>
    ///     Represent a FreeSwitch channel variable
    /// </summary>
    public sealed class EslChannelVariable {
        public EslChannelVariable(string name,
            string value) {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public override string ToString() {
            return Name + "=" + Value;
        }
    }
}