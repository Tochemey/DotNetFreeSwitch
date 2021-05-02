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

namespace DotNetFreeSwitch.Common
{
    public enum HangupCause
    {
        NONE = 0,
        UNALLOCATED_NUMBER = 1,
        NO_ROUTE_TRANSIT_NET = 2,
        NO_ROUTE_DESTINATION = 3,
        CHANNEL_UNACCEPTABLE = 6,
        CALL_AWARDED_DELIVERED = 7,
        NORMAL_CLEARING = 16,
        USER_BUSY = 17,
        NO_USER_RESPONSE = 18,
        NO_ANSWER = 19,
        SUBSCRIBER_ABSENT = 20,
        CALL_REJECTED = 21,
        NUMBER_CHANGED = 22,
        REDIRECTION_TO_NEW_DESTINATION = 23,
        EXCHANGE_ROUTING_ERROR = 25,
        DESTINATION_OUT_OF_ORDER = 27,
        INVALID_NUMBER_FORMAT = 28,
        FACILITY_REJECTED = 29,
        RESPONSE_TO_STATUS_ENQUIRY = 30,
        NORMAL_UNSPECIFIED = 31,
        NORMAL_CIRCUIT_CONGESTION = 34,
        NETWORK_OUT_OF_ORDER = 38,
        NORMAL_TEMPORARY_FAILURE = 41,
        SWITCH_CONGESTION = 42,
        ACCESS_INFO_DISCARDED = 43,
        REQUESTED_CHAN_UNAVAIL = 44,
        PRE_EMPTED = 45,
        FACILITY_NOT_SUBSCRIBED = 50,
        OUTGOING_CALL_BARRED = 52,
        INCOMING_CALL_BARRED = 54,
        BEARERCAPABILITY_NOTAUTH = 57,
        BEARERCAPABILITY_NOTAVAIL = 58,
        SERVICE_UNAVAILABLE = 63,
        BEARERCAPABILITY_NOTIMPL = 65,
        CHAN_NOT_IMPLEMENTED = 66,
        FACILITY_NOT_IMPLEMENTED = 69,
        SERVICE_NOT_IMPLEMENTED = 79,
        INVALID_CALL_REFERENCE = 81,
        INCOMPATIBLE_DESTINATION = 88,
        INVALID_MSG_UNSPECIFIED = 95,
        MANDATORY_IE_MISSING = 96,
        MESSAGE_TYPE_NONEXIST = 97,
        WRONG_MESSAGE = 98,
        IE_NONEXIST = 99,
        INVALID_IE_CONTENTS = 100,
        WRONG_CALL_STATE = 101,
        RECOVERY_ON_TIMER_EXPIRE = 102,
        MANDATORY_IE_LENGTH_ERROR = 103,
        PROTOCOL_ERROR = 111,
        INTERWORKING = 127,
        SUCCESS = 142,
        ORIGINATOR_CANCEL = 487,
        CRASH = 500,
        SYSTEM_SHUTDOWN = 501,
        LOSE_RACE = 502,
        MANAGER_REQUEST = 503,
        BLIND_TRANSFER = 600,
        ATTENDED_TRANSFER = 601,
        ALLOTTED_TIMEOUT = 602,
        USER_CHALLENGE = 603,
        MEDIA_TIMEOUT = 604,
        PICKED_OFF = 605,
        USER_NOT_REGISTERED = 606,
        PROGRESS_TIMEOUT = 607,
        UNKNOWN = 9999
    }
}