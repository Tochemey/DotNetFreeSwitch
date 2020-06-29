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

namespace Core.Commands
{
    /// <summary>
    ///     Helps Schedule some command to be executed.
    /// </summary>
    public sealed class SchedApiCommand : BaseCommand
    {
        private readonly bool _asynchronous;
        private readonly string _command;
        private readonly string _groupName;
        private readonly bool _repetitive;
        private readonly int _time;

        public SchedApiCommand(string command,
            string groupName,
            bool repetitive,
            int time,
            bool asynchronous)
        {
            _command = command;
            _groupName = groupName;
            _repetitive = repetitive;
            _time = time;
            _asynchronous = asynchronous;
        }

        protected override string Argument
        {
            get
            {
                var args = $"+{_time} {_groupName} {_command} {(_asynchronous ? "&" : string.Empty)}";
                if (_repetitive)
                    args = $"@{_time} {_groupName} {_command} {(_asynchronous ? "&" : string.Empty)}";
                return args;
            }
        }

        public override string Command => "sched_api";
    }
}