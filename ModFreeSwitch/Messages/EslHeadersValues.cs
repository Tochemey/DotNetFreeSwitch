namespace ModFreeSwitch.Messages {
    /// <summary>
    ///     FreeSwitch message header values
    /// </summary>
    public static class EslHeadersValues {
        /// <summary>
        ///     Often informs about the status of a command response
        /// </summary>
        public static string Ok = "+OK";

        /// <summary>
        ///     Only received when connecting to FreeSwitch for the first time. This header values helps track the authentication
        ///     request made by freeSwitch to allow smooth connection to the mod_event_socket.
        /// </summary>
        public static string AuthRequest = "auth/request";

        /// <summary>
        ///     Qualify an api command response
        /// </summary>
        public static string ApiResponse = "api/response";

        /// <summary>
        ///     General command response. It is used whenever a command is sent to FreeSwitch
        /// </summary>
        public static string CommandReply = "command/reply";

        /// <summary>
        ///     This header value helps track freeSwitch events
        /// </summary>
        public static string TextEventPlain = "text/event-plain";

        /// <summary>
        ///     Same as <see cref="TextEventPlain" />. However the freeSwitch events messages are sent in XML format.
        /// </summary>
        public static string TextEventXML = "text/event-xml";

        /// <summary>
        ///     Qualifies a disconnection from freeSwitch mod_event_socket
        /// </summary>
        public static string TextDisconnectNotice = "text/disconnect-notice";

        /// <summary>
        ///     In conjunction with <see cref="Ok" /> it helps to know the status of any command sent to freeSwitch via
        ///     mod_event_socket as an invalid command.
        /// </summary>
        public static string ErrInvalid = "-ERR invalid";

        /// <summary>
        ///     <see cref="ErrInvalid" />. Here the command sent results in errors.
        /// </summary>
        public static string Err = "-ERR";

        /// <summary>
        ///     Often observed during connection to freeSwitch mod_event_socket. It often happens when there is network issue.
        /// </summary>
        public static string TextRudeRejection = "text/rude-rejection";
    }
}