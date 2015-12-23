using System;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     SendMSG command. Help execute an application for a specific FreeSwitch channel
    /// </summary>
    public sealed class SendMsgCommand : BaseCommand {
        public const string CALL_COMMAND = "execute";
        private readonly string _applicationArgs;
        private readonly string _applicationName;
        private readonly string _callCommand;
        private readonly bool _eventLock;
        private readonly int _loop;
        private readonly Guid _uuid;

        public SendMsgCommand(Guid uuid, string callCommand, string applicationName, string applicationArgs, bool eventLock, int loop) {
            _uuid = uuid;
            _callCommand = callCommand;
            _applicationName = applicationName;
            _applicationArgs = applicationArgs;
            _eventLock = eventLock;
            _loop = loop;
        }

        public SendMsgCommand(string callCommand, string applicationName, string applicationArgs, bool eventLock, int loop) {
            _applicationName = applicationName;
            _applicationArgs = applicationArgs;
            _loop = loop;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _uuid = Guid.Empty;
        }

        public SendMsgCommand(Guid uniqueId, string callCommand, string applicationName, string applicationArgs, bool eventLock) {
            _uuid = uniqueId;
            _applicationName = applicationName;
            _applicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _loop = 1;
        }

        public SendMsgCommand(string callCommand, string applicationName, string applicationArgs, bool eventLock) {
            _applicationName = applicationName;
            _applicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = callCommand;
            _loop = 1;
            _uuid = Guid.Empty;
        }


        public SendMsgCommand(string applicationName, string applicationArgs, bool eventLock) {
            _applicationName = applicationName;
            _applicationArgs = applicationArgs;
            _eventLock = eventLock;
            _callCommand = CALL_COMMAND;
            _loop = 1;
            _uuid = Guid.Empty;
        }

        public override string Command
        {
            get
            {
                string cmd = string.Format("sendmsg  {0}\ncall-command: {1}\nexecute-app-name: {2}\nexecute-app-arg: {3}\nloops: {4}", _uuid, _callCommand, _applicationName, _applicationArgs, _loop);
                if (_eventLock) cmd += string.Format("\nevent-lock: {0}", "true");
                else cmd += string.Format("\nevent-lock: {0}", "false");
                return cmd;
            }
        }

        public override string Argument { get { return string.Empty; } }

        public string ApplicationName { get { return _applicationName; } }

        public string ApplicationArgs { get { return _applicationArgs; } }
    }
}