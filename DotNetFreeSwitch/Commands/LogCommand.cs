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

namespace DotNetFreeSwitch.Commands
{
   /// <summary>
   ///     FreeSwitch log
   /// </summary>
   public sealed class LogCommand : BaseCommand
   {
      /// <summary>
      ///     Log level
      /// </summary>
      private readonly LogLevels _logLevel;

      public LogCommand(LogLevels logLevel)
      {
         _logLevel = logLevel;
      }

      protected override string Argument => _logLevel.ToString();
      public override string CommandName => "log";
   }
}