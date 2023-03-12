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

using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Messages;

namespace DotNetFreeSwitch.Events
{
   public class ChannelStateEvent : Event
   {
      public ChannelStateEvent(Message message) : base(message)
      {
      }

      public string AnswerState => this["Answer-State"];

      public ChannelDirection CallDirection => EnumExtensions.Parse<ChannelDirection>(this["Call-Direction"]);

      public ChannelState ChannelState
      {
         get
         {
            var ch = this["Channel-State"];
            return string.IsNullOrEmpty(ch) ? ChannelState.UNKNOWN : EnumExtensions.Parse<ChannelState>(ch.Trim());
         }
      }

      public CallState CallState
      {
         get
         {
            var cs = this["Channel-Call-State"];
            return string.IsNullOrEmpty(cs) ? CallState.DOWN : EnumExtensions.Parse<CallState>(cs);
         }
      }

      public ChannelDirection PresenceCallDirection
      {
         get
         {
            var pcd = this["Presence-Call-Direction"];
            return string.IsNullOrEmpty(pcd)
                ? ChannelDirection.UNKNOWN
                : EnumExtensions.Parse<ChannelDirection>(this["Presence-Call-Direction"]);
         }
      }
   }
}