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
using DotNetFreeSwitch.Messages;

namespace DotNetFreeSwitch.Events
{
    public class Dtmf : FsEvent
    {
        public Dtmf(Message message) : base(message)
        {
        }

        public char Digit => Convert.ToChar(this["DTMF-Digit"]);

        public int Duration => int.TryParse(this["DTMF-Duration"],
            out var duration)
            ? duration
            : 0;

        public override string ToString()
        {
            return "Dtmf(" + Digit + ").";
        }
    }
}