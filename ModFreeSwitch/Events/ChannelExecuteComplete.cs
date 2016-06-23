using ModFreeSwitch.Messages;

namespace ModFreeSwitch.Events {
    public class ChannelExecuteComplete : EslEvent {
        public ChannelExecuteComplete(EslMessage message) : base(message) {}

        public string Application {
            get { return this["Application"]; }
        }

        public string ApplicationData {
            get { return this["Application-Data"]; }
        }

        /// <summary>
        ///     Gets reponse from the application
        /// </summary>
        public string ApplicationResponse {
            get { return this["Application-Response"]; }
        }

        public override string ToString() {
            return "ExecuteComplete(" + Application + ", '" + ApplicationData + "')." +
                   base.ToString();
        }
    }
}