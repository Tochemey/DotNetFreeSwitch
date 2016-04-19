using System;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Commands {
    public abstract class BaseCommand : IEquatable<BaseCommand> {
        protected BaseCommand() { Sequence = GuidFactory.Create().ToString(); }

        /// <summary>
        ///     The command name
        /// </summary>
        public abstract string Command { get; }

        /// <summary>
        ///     The command argument
        /// </summary>
        public abstract string Argument { get; }

        /// <summary>
        ///     Additional Data to add to the command
        /// </summary>
        public object Optional { set; get; }

        /// <summary>
        ///     Command Reply Message. Some command needs reply
        /// </summary>
        public EslMessage CommandReply { set; get; }

        /// <summary>
        ///     Command sequence number
        /// </summary>
        public string Sequence { private set; get; }

        public bool Equals(BaseCommand other) {
            if (other == null) return false;
            if (ToString().Equals(other.ToString())
                && Sequence == other.Sequence) return true;
            return false;
        }

        public override string ToString() { return $"{Command} {Argument}"; }
    }
}