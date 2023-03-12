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
   ///     SetVar helps set a variable for a specific FreeSwitch channel using the ApiCommand
   /// </summary>
   public sealed class SetVarCommand : BaseCommand
   {
      /// <summary>
      ///     Channel Id
      /// </summary>
      private readonly string _uuid;

      public SetVarCommand(string uuid,
          string name,
          string value)
      {
         _uuid = uuid;
         Name = name;
         Value = value;
      }

      protected override string Argument => $"{_uuid} {Name} {Value}";

      public override string CommandName => "uuid_setvar";

      /// <summary>
      ///     Variable name
      /// </summary>
      public string Name { get; }

      /// <summary>
      ///     Variable value
      /// </summary>
      public string Value { get; }
   }
}