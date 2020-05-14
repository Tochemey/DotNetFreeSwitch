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

namespace Core.Common
{
    public enum EventType
    {
        CUSTOM,
        CHANNEL_CREATE,
        CHANNEL_DESTROY,
        CHANNEL_DATA,
        CHANNEL_STATE,
        CHANNEL_ANSWER,
        CHANNEL_APPLICATION,
        CHANNEL_HANGUP,
        CHANNEL_HANGUP_COMPLETE,
        CHANNEL_EXECUTE,
        CHANNEL_EXECUTE_COMPLETE,
        CHANNEL_BRIDGE,
        CHANNEL_UNBRIDGE,
        CHANNEL_PROGRESS,
        CHANNEL_PROGRESS_MEDIA,
        CHANNEL_ORIGINATE,
        CHANNEL_OUTGOING,
        CHANNEL_PARK,
        CHANNEL_UNPARK,
        CALL_UPDATE,
        CHANNEL_CALLSTATE,
        CHANNEL_UUID,
        API,
        BACKGROUND_JOB,
        RE_SCHEDULE,
        LOG,
        INBOUND_CHAN,
        OUTBOUND_CHAN,
        STARTUP,
        SHUTDOWN,
        SESSION_HEARTBEAT,
        PUBLISH,
        UNPUBLISH,
        TALK,
        NOTALK,
        SESSION_CRASH,
        MODULE_LOAD,
        DTMF,
        MESSAGE,
        PRESENCE_IN,
        PRESENCE_OUT,
        PRESENCE_PROBE,
        SOFIA_REGISTER,
        SOFIA_EXPIRES,
        CODEC,
        DETECTED_SPEECH,
        PRIVATE_COMMAND,
        HEARTBEAT,
        ALL,
        MODULE_UNLOAD,
        RELOADXML,
        NOTIFY,
        SEND_MESSAGE,
        RECV_MESSAGE,
        REQUEST_PARAMS,
        GENERAL,
        COMMAND,
        CLIENT_DISCONNECTED,
        SERVER_DISCONNECTED,
        SEND_INFO,
        RECV_INFO,
        CALL_SECURE,
        NAT,
        RECORD_START,
        RECORD_STOP,
        PLAYBACK_START,
        PLAYBACK_STOP,
        DETECTED_TONE,
        MESSAGE_WAITING,
        MESSAGE_QUERY,
        ROSTER,
        RECV_RTCP_MESSAGE,
        ADD_SCHEDULE,
        DEL_SCHEDULE,
        EXE_SCHEDULE,
        TRAP
    }
}
