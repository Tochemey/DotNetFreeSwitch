namespace ModFreeSwitch.Messages {
    /// <summary>
    /// Command/Reply message
    /// </summary>
    public sealed class CommandReplyMessage {
        public CommandReplyMessage(EventSocketMessage response) {
            Response = response;
            ReplyText = response != null ? response.HeaderValue(EventSocketHeaders.ReplyText) : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) && ReplyText.StartsWith(EventSocketHeadersValues.Ok);
        }

        /// <summary>
        ///     CommandReply response
        /// </summary>
        public EventSocketMessage Response { get; private set; }

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