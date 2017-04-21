/*
    Copyright [2016] [Arsene Tochemey GANDOTE]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;

namespace ModFreeSwitch
{
    /// <summary>
    ///     Entry point to connect to a running FreeSWITCH Event Socket Library module, as client
    ///     This class provides what the FreeSWITCH documentation refers to as an 'Inbound' connection to the Event Socket
    ///     module. That is, with reference to the socket listening on the FreeSWITCH server, this client occurs as an inbound
    ///     connection to the server. See
    ///     <a href="http://wiki.freeswitch.org/wiki/Mod_event_socket">http://wikifreeswitch.org/wiki/Mod_event_socket</a>
    /// </summary>
    public class EventSocketClient
    {
        private static MultithreadEventLoopGroup group;
        private static Bootstrap bootstrap;
    }
}