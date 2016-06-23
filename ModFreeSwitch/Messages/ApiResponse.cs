namespace ModFreeSwitch.Messages {
    public sealed class ApiResponse {
        public ApiResponse(string command,
            EslMessage response) {
            Command = command;
            var response1 = response;
            ReplyText = response1 != null
                ? response1.HeaderValue(EslHeaders.ReplyText)
                : string.Empty;
            IsOk = !string.IsNullOrEmpty(ReplyText) &&
                   ReplyText.StartsWith(EslHeadersValues.Ok);
        }

        public string Command { get; private set; }

        /// <summary>
        ///     Actual reply text
        /// </summary>
        public string ReplyText { get; }

        /// <summary>
        ///     Check whether the command has been successful or not.
        /// </summary>
        public bool IsOk { get; private set; }
    }
}