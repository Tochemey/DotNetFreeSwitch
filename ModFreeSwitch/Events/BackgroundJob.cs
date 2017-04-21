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

using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events
{
    /// <summary>
    ///     Result for a background job.
    /// </summary>
    public class BackgroundJob : EslEvent
    {
        public BackgroundJob(EslMessage message) : base(message) { }

        /// <summary>
        ///     Gets ID of the bgapi job
        /// </summary>
        public string JobUid => this["Job-UUID"];

        /// <summary>
        ///     Gets command which was executed
        /// </summary>
        public string CommandName => this["Job-Command"];

        /// <summary>
        ///     Gets arguments for the command
        /// </summary>
        public string CommandArguments => this["Job-Command-Arg"];

        /// <summary>
        ///     Gets the actual command result.
        /// </summary>
        public string CommandResult
        {
            get
            {
                var commandResult = this["__CONTENT__"];
                return commandResult;
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CommandName + "(" + CommandArguments + ") = '" + CommandResult + "'\r\n\t";
        }
    }
}