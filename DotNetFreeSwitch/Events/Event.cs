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

using System;
using System.Text;
using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Messages;

namespace DotNetFreeSwitch.Events
{
   /// <summary>
   /// Event represents the FS event
   /// </summary>
   public class Event
   {
      private readonly bool _ignoreBody;
      private readonly Message _response;

      /// <summary>
      /// Initialize the Event class
      /// </summary>
      /// <param name="response">the freeswitch message</param>
      /// <param name="ignoreBody">states whether to ignore the message body or not</param>
      public Event(Message response,
          bool ignoreBody = false)
      {
         _response = response;
         _ignoreBody = ignoreBody;
      }

      /// <summary>
      /// Specifies the caller unique id
      /// </summary>
      /// <returns></returns>
      public Guid CallerGuid => Guid.Parse(this["Caller-Unique-ID"]);

      /// <summary>
      /// Specifies the channel unique id
      /// </summary>
      /// <returns></returns>
      public Guid ChannelCallGuid => Guid.Parse(this["Channel-Call-UUID"]);

      /// <summary>
      /// Specifies the channel name
      /// </summary>
      public string ChannelName => this["Channel-Name"];

      /// <summary>
      /// Specifies the core unique id
      /// </summary>
      /// <returns></returns>
      public Guid CoreGuid => Guid.Parse(this["Core-UUID"]);

      /// <summary>
      /// Specifies the freeswitch calling file
      /// </summary>
      public string EventCallingFile => this["Event-Calling-File"];

      /// <summary>
      /// Specifies the freeswitch calling function
      /// </summary>
      public string EventCallingFunction => this["Event-Calling-Function"];

      /// <summary>
      /// Specifies the calling line number
      /// </summary>
      public string EventCallingLineNumber => this["Event-Calling-Line-Number"];

      /// <summary>
      /// Specifies the event date in GMT
      /// </summary>
      /// <returns></returns>
      public DateTime EventDateGmt => DateTime.Parse(this["Event-Date-GMT"]);

      /// <summary>
      /// Specifies the event local date
      /// </summary>
      /// <returns></returns>
      public DateTime EventDateLocal => DateTime.Parse(this["Event-Date-Local"]);

      /// <summary>
      /// Specifies the event name
      /// </summary>
      public string EventName => this["Event-Name"];

      /// <summary>
      /// Specifies the event sub class
      /// </summary>
      public string EventSubClass => this["Event-Subclass"];

      /// <summary>
      /// Specifies the event timestamp
      /// </summary>
      /// <returns></returns>
      public DateTime EventTimeStamp => this["Event-Date-timestamp"].FromUnixTime();

      /// <summary>
      /// Specifies the freeswitch hostname
      /// </summary>
      public string HostName => this["FreeSWITCH-Hostame"];

      /// <summary>
      /// Specifies the freeswitch ipv4 address
      /// </summary>
      public string IPv4 => this["FreeSWITCH-IPv4"];

      /// <summary>
      /// Specifies the unique id
      /// </summary>
      /// <returns></returns>
      public Guid UniqueId => Guid.Parse(this["Unique-ID"]);

      public string this[string headerName]
      {
         get
         {
            if (_ignoreBody)
            {
               if (_response.HasHeader(headerName)) return _response.HeaderValue(headerName);
               if (_response.HasHeader("variable_" + headerName))
                  return _response.HeaderValue("variable_" + headerName);
            }

            var map = _response.ParseBodyLines();
            if (map.ContainsKey(headerName)) return map[headerName];
            return map.ContainsKey("variable_" + headerName) ? map["variable_" + headerName] : null;
         }
      }

      //protected Dictionary<string, string> ParseBodyLines() {
      //    var resp = new Dictionary<string, string>();
      //    foreach (var bodyLine in _response.BodyLines) {
      //        var parsedLines = EslHeaderParser.SplitHeader(bodyLine);
      //        if (parsedLines == null) continue;
      //        if (parsedLines.Length == 2) resp.Add(parsedLines[0], parsedLines[1]);
      //        if (parsedLines.Length == 1) resp.Add("__CONTENT__", bodyLine);
      //    }
      //    return resp;
      //}

      public override string ToString()
      {
         var sb = new StringBuilder();
         if (_ignoreBody)
         {
            foreach (var str in _response.Headers.Keys) sb.AppendLine(str + ":" + _response.Headers[str]);
            return sb.ToString();
         }

         var map = _response.ParseBodyLines();
         foreach (var str in map.Keys) sb.AppendLine(str + ":" + map[str]);
         return sb.ToString();
      }
   }
}