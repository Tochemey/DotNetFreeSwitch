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

namespace Core.Commands
{
    /// <summary>
    ///     SendMSG command. Help execute an application for a specific FreeSwitch channel
    /// </summary>
    public sealed class SendMsgCommand : BaseCommand
    {
        public const string CallCommand = "execute";
        private readonly string _callCommand;
        private readonly bool _eventLock;
        private readonly int _loop;
        private readonly Guid _uuid;

        public SendMsgCommand(Guid uuid,
            string callCommand,
            string applicationName,
            string applicationArgs,
            bool eventLock,
            int loop)
        {
            _uuid = uuid;
            _callCommand = callCommand;
            ApplicationName = applicationName;
            ApplicationArgs = applicationArgs;
            _eventLock = eventLock;
            _loop = loop;
        }

        public SendMsgCommand(string callCommand,
            string applicationName,
            string applicationArgs,
            bool eventLock,
            int loop)
        {
            ApplicationName = applicationName;
            ApplicationArgs = applicationArgs;
            _loop = loop;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _uuid = Guid.Empty;
        }

        public SendMsgCommand(Guid uniqueId,
            string callCommand,
            string applicationName,
            string applicationArgs,
            bool eventLock)
        {
            _uuid = uniqueId;
            ApplicationName = applicationName;
            ApplicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _loop = 1;
        }

        public SendMsgCommand(string callCommand,
            string applicationName,
            string applicationArgs,
            bool eventLock)
        {
            ApplicationName = applicationName;
            ApplicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _loop = 1;
            _uuid = Guid.Empty;
        }

        public SendMsgCommand(string applicationName,
            string applicationArgs,
            bool eventLock)
        {
            ApplicationName = applicationName;
            ApplicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = CallCommand;
            _loop = 1;
            _uuid = Guid.Empty;
        }

        public string ApplicationArgs { get; }

        public string ApplicationName { get; }

        protected override string Argument => string.Empty;

        public override string Command
        {
            get
            {
                var cmd =
                    $"sendmsg  {_uuid}\ncall-command: {_callCommand}\nexecute-app-name: {ApplicationName}\nexecute-app-arg: {ApplicationArgs}\nloops: {_loop}";
               
                if (_eventLock) cmd += "\nevent-lock: true";
                else cmd += "\nevent-lock: false";
                return cmd;
            }
        }
    }
}