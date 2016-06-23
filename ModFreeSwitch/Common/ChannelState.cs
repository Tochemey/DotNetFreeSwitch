namespace ModFreeSwitch.Common {
    public enum ChannelState {
        /// <summary>
        ///     Channel is created
        /// </summary>
        CS_NEW,

        /// <summary>
        ///     Initialized
        /// </summary>
        CS_INIT,

        /// <summary>
        ///     Going through dial plan
        /// </summary>
        CS_ROUTING,

        /// <summary>
        ///     Passive transmit state
        /// </summary>
        CS_SOFT_EXECUTE,

        /// <summary>
        ///     Executing the dial plan
        /// </summary>
        CS_EXECUTE,

        /// <summary>
        ///     Connected to another channel
        /// </summary>
        CS_EXCHANGE_MEDIA,

        /// <summary>
        ///     Being parked (not same as held)
        /// </summary>
        CS_PARK,

        /// <summary>
        ///     Sending media (as .wav) to channel
        /// </summary>
        CS_CONSUME_MEDIA,

        /// <summary>
        ///     Channel is sleeping
        /// </summary>
        CS_HIBERNATE,

        /// <summary>
        ///     Channel is being reset.
        /// </summary>
        CS_RESET,

        /// <summary>
        ///     Flagged for hangup but not yet terminated.
        /// </summary>
        CS_HANGUP,

        /// <summary>
        ///     Flag is done and ready to be destroyed.
        /// </summary>
        CS_DONE,

        /// <summary>
        ///     Remove the channel
        /// </summary>
        /// &
        CS_DESTROY,

        CS_REPORTING,

        /// <summary>
        ///     Unknown state.
        /// </summary>
        UNKNOWN
    }
}