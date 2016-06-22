namespace ModFreeSwitch.Messages {
    /// <summary>
    /// Command/Reply message
    /// </summary>
    public sealed class CommandReply {
        public CommandReply(string command, EslMessage response) {
            Command = command;
            _response = response;
            ReplyText = _response != null ? _response.HeaderValue(EslHeaders.ReplyText) : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) && ReplyText.StartsWith(EslHeadersValues.Ok);
        }

        /// <summary>
        ///     CommandReply response
        /// </summary>
        private readonly EslMessage _response;

        public string Command { get; private set; }

        /// <summary>
        ///     Actual reply text
        /// </summary>
        public string ReplyText { get; private set; }

        /// <summary>
        ///     Check whether the command has been successful or not.
        /// </summary>
        public bool IsOk { get; private set; }

        public string this[string headerName]
        {
            get { return _response.HeaderValue(headerName); }
        }

        /// <summary>
        /// Returns the actual command/reply type
        /// </summary>
        public string ContentType {
            get { return _response.ContentType(); }
        }
    }
}