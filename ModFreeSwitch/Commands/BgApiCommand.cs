using System;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     BgApi Command. It helps to execute other command in a background job on FreeSwitch
    /// </summary>
    public sealed class BgApiCommand : BaseCommand {
        /// <summary>
        ///     The command to execute Arguments
        /// </summary>
        private readonly string _commandArgs;

        /// <summary>
        ///     The command to execute name
        /// </summary>
        private readonly string _commandName;

        public BgApiCommand(string commandName,
            string commandArgs) {
            _commandName = commandName;
            _commandArgs = commandArgs;
        }

        /// <summary>
        ///     Each command send by BgApi will have a trackable Id that will helps identify which one has sent a response.
        /// </summary>
        public Guid CommandId { get; set; }

        public string CommandName {
            get { return _commandName; }
        }

        public string CommandArgs {
            get { return _commandArgs; }
        }

        /// <summary>
        ///     The BgApi command
        /// </summary>
        public override string Command {
            get { return "bgapi"; }
        }

        /// <summary>
        ///     The BgApi command argument
        /// </summary>
        public override string Argument {
            get { return string.Format("{0} {1}", CommandName, CommandArgs); }
        }
    }
}