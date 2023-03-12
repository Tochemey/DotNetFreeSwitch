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
using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Messages;

namespace DotNetFreeSwitch.Events
{
   public class RecordStop : Event
   {
      public RecordStop(Message message) : base(message)
      {
      }

      public string RecordFilePath => this["Record-File-Path"];

      public int RecordMilliSeconds => string.IsNullOrEmpty(this["record_ms"]) ? 0 :
          this["record_ms"].IsNumeric() ? Convert.ToInt32(this["record_ms"]) : 0;

      public int RecordSeconds => string.IsNullOrEmpty(this["record_seconds"]) ? 0 :
          this["record_seconds"].IsNumeric() ? Convert.ToInt32(this["record_seconds"]) : 0;
   }
}