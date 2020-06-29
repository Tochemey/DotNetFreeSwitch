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
using System.Collections.Generic;
using System.Text;
using Core.Commands;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;

namespace Core.Codecs
{
    /// <summary>
    ///     Helps to encode the command that will be sent to freeSwitch via the mod_event_socket. All it does is to append
    ///     double Line Feed character at the end every command that goes against freeSwitch.
    /// </summary>
    public sealed class FrameEncoder : MessageToMessageEncoder<BaseCommand>
    {
        private const string MessageEndString = "\n\n";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Encoding _encoding;

        public FrameEncoder(Encoding encoding) { _encoding = encoding; }

        public FrameEncoder() : this(Encoding.GetEncoding(0)) { }

        public override bool IsSharable => true;

        protected override void Encode(IChannelHandlerContext context,
            BaseCommand message,
            List<object> output)
        {
            // Let us get the string representation of the message sent
            if (string.IsNullOrEmpty(message?.ToString())) return;
            
            var msg = message.ToString().Trim();

            if (!msg.Trim().EndsWith(MessageEndString, StringComparison.Ordinal)) 
                msg += MessageEndString;
            
            if (Logger.IsDebugEnabled)
                Logger.Debug("Encoded message sent [{0}]",
                    msg.Trim());

            output.Add(ByteBufferUtil.EncodeString(context.Allocator,
                msg,
                _encoding));
        }
    }
}
