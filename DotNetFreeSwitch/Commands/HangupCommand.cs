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

namespace DotNetFreeSwitch.Commands
{
    /// <summary>
    ///     Helps to send an explicit hangup to a call with a specific reason.
    /// </summary>
    public sealed class HangupCommand : BaseCommand
    {
        public const string CallCommand = "hangup";

        /// <summary>
        ///     The hangup cause
        /// </summary>
        private readonly string _reason;

        /// <summary>
        ///     The call id
        /// </summary>
        private readonly Guid _uuid;

        public HangupCommand(Guid uuid,
            string reason)
        {
            _uuid = uuid;
            _reason = reason;
        }

        protected override string Argument => string.Empty;

        public override string Command => $"sendmsg  {_uuid}\ncall-command: {CallCommand}\nhangup-cause: {_reason}";
    }
}