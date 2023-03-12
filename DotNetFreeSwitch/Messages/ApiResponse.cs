using System;
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

using System.Linq;

namespace DotNetFreeSwitch.Messages
{
   public sealed class ApiResponse
   {
      public ApiResponse(string command,
          Message response)
      {
         Command = command;
         var reply = response;
         ReplyText = reply != null ? reply.BodyLines.First() : string.Empty;
         IsOk = !string.IsNullOrEmpty(ReplyText) && ReplyText.StartsWith(HeadersValues.Ok);
      }

      public string Command { get; }

      /// <summary>
      ///     Actual reply text
      /// </summary>
      public string ReplyText { get; }

      /// <summary>
      ///     Check whether the command has been successful or not.
      /// </summary>
      public bool IsOk { get; }
   }
}