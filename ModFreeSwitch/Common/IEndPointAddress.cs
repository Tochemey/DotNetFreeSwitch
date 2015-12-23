namespace ModFreeSwitch.Common {
    /// <summary>
    ///     An address for an FreeSWITCH end point
    /// </summary>
    /// <remarks>Can be a SIP address, an sofia address or event an application</remarks>
    public interface IEndPointAddress {
        /// <summary>
        ///     Format the address as a string which could be dialed using the "originate" or "bridge" commands
        /// </summary>
        /// <returns>Properly formatted string</returns>
        string ToDialString();
    }
}