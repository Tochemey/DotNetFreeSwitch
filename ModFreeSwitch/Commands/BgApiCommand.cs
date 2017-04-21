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

using System;

namespace ModFreeSwitch.Commands
{
    /// <summary>
    ///     BgApi Command. It helps to execute other command in a background job on FreeSwitch
    /// </summary>
    public sealed class BgApiCommand : BaseCommand
    {
        /// <summary>
        ///     The command to execute Arguments
        /// </summary>
        private readonly string _commandArgs;

        /// <summary>
        ///     The command to execute name
        /// </summary>
        private readonly string _commandName;

        public BgApiCommand(string commandName,
            string commandArgs)
        {
            _commandName = commandName;
            _commandArgs = commandArgs;
        }

        /// <summary>
        ///     Each command send by BgApi will have a trackable Id that will helps identify which one has sent a response.
        /// </summary>
        public Guid CommandId { get; set; }

        public string CommandName => _commandName;

        public string CommandArgs => _commandArgs;

        /// <summary>
        ///     The BgApi command
        /// </summary>
        public override string Command => "bgapi";

        /// <summary>
        ///     The BgApi command argument
        /// </summary>
        public override string Argument => string.Format("{0} {1}",
            CommandName,
            CommandArgs);
    }
}