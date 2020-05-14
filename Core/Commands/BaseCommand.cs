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
using Core.Common;
using Core.Messages;

namespace Core.Commands
{
    public abstract class BaseCommand : IEquatable<BaseCommand>
    {
        protected BaseCommand() => Sequence = GuidFactory.Create().ToString();

        /// <summary>
        /// The command argument 
        /// </summary>
        public abstract string Argument { get; }

        /// <summary>
        /// The command name 
        /// </summary>
        public abstract string Command { get; }

        /// <summary>
        /// Command Reply Message. Some command needs reply 
        /// </summary>
        public FsMessage CommandReply { set; get; }

        /// <summary>
        /// Additional Data to add to the command 
        /// </summary>
        public object Optional { set; get; }

        /// <summary>
        /// Command sequence number 
        /// </summary>
        public string Sequence { get; }

        public bool Equals(BaseCommand other)
        {
            if (other == null) return false;
            return ToString().Equals(other.ToString()) && Sequence == other.Sequence;
        }

        public override string ToString() => $"{Command} {Argument}";
    }
}