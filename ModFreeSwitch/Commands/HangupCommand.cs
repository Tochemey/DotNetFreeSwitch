using System;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Helps to send an explicit hangup to a call with a specific reason.
    /// </summary>
    public sealed class HangupCommand : BaseCommand {
        public const string CALL_COMMAND = "hangup";

        /// <summary>
        ///     The call id
        /// </summary>
        private readonly Guid _uuid;

        /// <summary>
        ///     The hangup cause
        /// </summary>
        private readonly string _reason;

        public HangupCommand(Guid uuid, string reason) {
            _uuid = uuid;
            _reason = reason;
        }

        public override string Command { get { return string.Format("sendmsg  {0}\ncall-command: {1}\nhangup-cause: {2}", _uuid, CALL_COMMAND, _reason); } }

        public override string Argument { get { return string.Empty; } }
    }
}