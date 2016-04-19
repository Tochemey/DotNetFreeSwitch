namespace ModFreeSwitch.Messages {
    /// <summary>
    /// Command/Reply message
    /// </summary>
    public sealed class CommandReplyMessage {
        public CommandReplyMessage(string command, EslMessage response) {
            Command = command;
            Response = response;
            ReplyText = response != null ? response.HeaderValue(EslHeaders.ReplyText) : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) && ReplyText.StartsWith(EslHeadersValues.Ok);
        }

        /// <summary>
        ///     CommandReply response
        /// </summary>
        public EslMessage Response { get; private set; }

        public string Command { get; private set; }

        /// <summary>
        ///     Actual reply text
        /// </summary>
        public string ReplyText { get; private set; }

        /// <summary>
        ///     Check whether the command has been successful or not.
        /// </summary>
        public bool IsOk { get; private set; }
    }
}