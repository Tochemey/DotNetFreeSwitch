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

namespace DotNetFreeSwitch.Commands
{
   /// <summary>
   ///     Helps to listen a specific channel events.
   /// </summary>
   public sealed class MyEventsCommand : BaseCommand
   {
      /// <summary>
      ///     Channel Id
      /// </summary>
      private readonly Guid _uuid;

      public MyEventsCommand(Guid uuid)
      {
         _uuid = uuid;
      }

      protected override string Argument => _uuid.ToString();
      public override string CommandName => "myevents";
   }
}