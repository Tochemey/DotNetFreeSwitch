namespace ModFreeSwitch.Messages {
    /// <summary>
    ///     Command/Reply message
    /// </summary>
    public sealed class CommandReply {
        public CommandReply(string command,
            EslMessage response) {
            Command = command;
            Response = response;
            ReplyText = Response != null
                ? Response.HeaderValue(EslHeaders.ReplyText)
                : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) &&
                   ReplyText.StartsWith(EslHeadersValues.Ok);
        }

        public string Command { get; private set; }

        public EslMessage Response { get; }

        /// <summary>
        ///     Actual reply text
        /// </summary>
        public string ReplyText { get; }

        /// <summary>
        ///     Check whether the command has been successful or not.
        /// </summary>
        public bool IsOk { get; private set; }

        public string this[string headerName] => Response.HeaderValue(headerName);

        /// <summary>
        ///     Returns the actual command/reply type
        /// </summary>
        public string ContentType => Response.ContentType();
    }
}