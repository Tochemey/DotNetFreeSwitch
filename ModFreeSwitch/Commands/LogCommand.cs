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

using ModFreeSwitch.Common;

namespace ModFreeSwitch.Commands
{
    /// <summary>
    ///     FreeSwitch log
    /// </summary>
    public sealed class LogCommand : BaseCommand
    {
        /// <summary>
        ///     Log level
        /// </summary>
        private readonly EslLogLevels _logLevel;

        public LogCommand(EslLogLevels logLevel) { _logLevel = logLevel; }

        public override string Command => "log";

        public override string Argument => _logLevel.ToString();
    }
}