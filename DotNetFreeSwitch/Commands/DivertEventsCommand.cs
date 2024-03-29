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

namespace DotNetFreeSwitch.Commands
{
   /// <summary>
   ///     DivertEvent
   /// </summary>
   public sealed class DivertEventsCommand : BaseCommand
   {
      private readonly bool _flag;

      public DivertEventsCommand(bool flag)
      {
         _flag = flag;
      }

      protected override string Argument => _flag ? "on" : "off";
      public override string CommandName => "divert_events";
   }
}