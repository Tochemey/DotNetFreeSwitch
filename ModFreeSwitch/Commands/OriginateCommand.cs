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
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModFreeSwitch.Commands
{
    /// <summary>
    /// originate 
    /// </summary>
    public sealed class OriginateCommand : BaseCommand
    {
        private readonly IEndPointAddress _caller;
        private readonly string _callerIdName;
        private readonly string _callerIdNumber;
        private readonly IList<EslChannelVariable> _channelVariables;
        private readonly string _context;
        private readonly IEndPointAddress _destination;
        private readonly string _dialplan;
        private readonly int _timeout;

        public OriginateCommand(IEndPointAddress caller,
            IEndPointAddress destination,
            string dialplan,
            string context,
            string callerIdName,
            string callerIdNumber,
            int timeout)
        {
            _caller = caller;
            _destination = destination;
            _dialplan = dialplan;
            _context = context;
            _callerIdName = callerIdName;
            _callerIdNumber = callerIdNumber;
            _timeout = timeout;
            _channelVariables = new List<EslChannelVariable>();
        }

        public OriginateCommand(IEndPointAddress caller,
            IEndPointAddress destination)
        {
            _caller = caller;
            _destination = destination;
            _dialplan = "XML";
            _context = "default";
            _callerIdName = string.Empty;
            _callerIdNumber = string.Empty;
            _timeout = 0;
        }

        public override string Argument
        {
            get
            {
                SetChannelVariable("origination_uuid",
                    Id.ToString());
                SetChannelVariable("ignore_early_media",
                    "true");
                SetChannelVariable("enable_heartbeat_events",
                    Heartbeat.ToString());
                var variables = _channelVariables != null && _channelVariables.Count > 0 ? _channelVariables.Aggregate(string.Empty,
                    (current,
                        variable) => current + (variable + ",")) : string.Empty;
                if (variables.Length <= 0) return $"{variables}{_caller.ToDialString()} {_destination.ToDialString()} {_dialplan} {_context} {_callerIdName} {_callerIdNumber} {_timeout}";
                if (string.IsNullOrEmpty(Option))
                    variables = "{" + variables.Remove(variables.Length - 1,
                                    1) + "}";
                else
                    variables = "{" + Option + ", " + variables.Remove(variables.Length - 1,
                                    1) + "}";
                return $"{variables}{_caller.ToDialString()} {_destination.ToDialString()} {_dialplan} {_context} {_callerIdName} {_callerIdNumber} {_timeout}";
            }
        }

        public override string Command => "originate";

        /// <summary>
        /// Call session heartbeat 
        /// </summary>
        public int Heartbeat { set; get; }

        public Guid Id { get; set; }

        /// <summary>
        /// Optional attribute like sip headers. 
        /// </summary>
        public string Option { set; get; }

        public void SetChannelVariable(string variable,
            string value)
        {
            var var = new EslChannelVariable(variable,
                value);
            _channelVariables.Add(var);
        }
    }
}