using System;
using System.Collections.Generic;
using System.Linq;
using ModFreeSwitch.Common;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     originate
    /// </summary>
    public sealed class OriginateCommand : BaseCommand {
        public Guid Id { get; set; }
        private readonly IList<EventSocketChannelVariable> _channelVariables;
        private readonly IEndPointAddress _caller;
        private readonly IEndPointAddress _destination;
        private readonly string _dialplan;
        private readonly string _context;
        private readonly string _callerIdName;
        private readonly string _callerIdNumber;
        private readonly int _timeout;

        public OriginateCommand(IEndPointAddress caller, IEndPointAddress destination, string dialplan, string context, string callerIdName, string callerIdNumber, int timeout) {
            _caller = caller;
            _destination = destination;
            _dialplan = dialplan;
            _context = context;
            _callerIdName = callerIdName;
            _callerIdNumber = callerIdNumber;
            _timeout = timeout;
            _channelVariables = new List<EventSocketChannelVariable>();
        }

        public OriginateCommand(IEndPointAddress caller, IEndPointAddress destination) {
            _caller = caller;
            _destination = destination;
            _dialplan = "XML";
            _context = "default";
            _callerIdName = string.Empty;
            _callerIdNumber = string.Empty;
            _timeout = 0;
        }

        /// <summary>
        ///     Optional attribute like sip headers.
        /// </summary>
        public string Option { set; get; }

        /// <summary>
        ///     Call session heartbeat
        /// </summary>
        public int Heartbeat { set; get; }

        public void SetChannelVariable(string variable, string value) {
            EventSocketChannelVariable var = new EventSocketChannelVariable(variable, value);
            _channelVariables.Add(var);
        }

        public override string Command { get { return "originate"; } }

        public override string Argument
        {
            get
            {
                SetChannelVariable("origination_uuid", Id.ToString());
                SetChannelVariable("ignore_early_media", "true");
                SetChannelVariable("enable_heartbeat_events", Heartbeat.ToString());
                string variables = (_channelVariables != null && _channelVariables.Count > 0) ? _channelVariables.Aggregate(string.Empty, (current, variable) => current + (variable + ",")) : string.Empty;
                if (variables.Length > 0) {
                    if (string.IsNullOrEmpty(Option)) variables = "{" + variables.Remove(variables.Length - 1, 1) + "}";
                    else variables = "{" + Option + ", " + variables.Remove(variables.Length - 1, 1) + "}";
                }
                return string.Format("{0}{1} {2} {3} {4} {5} {6} {7}", variables, _caller.ToDialString(), _destination.ToDialString(), _dialplan, _context, _callerIdName, _callerIdNumber, _timeout);
            }
        }
    }
}