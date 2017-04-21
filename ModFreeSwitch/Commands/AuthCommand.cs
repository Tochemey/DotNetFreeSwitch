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

namespace ModFreeSwitch.Commands
{
    /// <summary>
    ///     The auth command helps to authenticate against FreeSwitch Event Socket module.
    /// </summary>
    public sealed class AuthCommand : BaseCommand
    {
        /// <summary>
        ///     The authentication password
        /// </summary>
        private readonly string _password;

        public AuthCommand(string password) { _password = password; }

        /// <summary>
        ///     Auth Command
        /// </summary>
        public override string Command => "auth";

        /// <summary>
        ///     Auth command argument
        /// </summary>
        public override string Argument => _password;
    }
}