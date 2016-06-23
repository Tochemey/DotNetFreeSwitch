using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;

namespace ModFreeSwitch {
    /// <summary>
    ///     Entry point to connect to a running FreeSWITCH Event Socket Library module, as client
    ///     This class provides what the FreeSWITCH documentation refers to as an 'Inbound' connection to the Event Socket
    ///     module. That is, with reference to the socket listening on the FreeSWITCH server, this client occurs as an inbound
    ///     connection to the server. See
    ///     <a href="http://wiki.freeswitch.org/wiki/Mod_event_socket">http://wikifreeswitch.org/wiki/Mod_event_socket</a>
    /// </summary>
    public class EventSocketClient {
        private static MultithreadEventLoopGroup group;
        private static Bootstrap bootstrap;
    }
}