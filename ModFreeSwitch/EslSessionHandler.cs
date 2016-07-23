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
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Messages;

namespace ModFreeSwitch {
    public class EslSessionHandler : ChannelHandlerAdapter {
        protected readonly Queue<CommandAsyncEvent> CommandAsyncEvents;

        public EslSessionHandler() {
            CommandAsyncEvents = new Queue<CommandAsyncEvent>();
        }

        public async Task<ApiResponse> SendApiAsync(ApiCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            CommandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as ApiResponse;
        }

        public async Task<Guid> SendBgApiAsync(BgApiCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            var jobUuid = Guid.Empty;
            CommandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            var reply = await asyncEvent.Task as CommandReply;
            if (reply == null) return jobUuid;
            if (reply.IsOk)
                return Guid.TryParse(reply[EslHeaders.JobUuid], out jobUuid)
                    ? jobUuid
                    : Guid.Empty;
            return jobUuid;
        }

        public async Task<CommandReply> SendCommandAsync(BaseCommand command,
            IChannel context) {
            var asyncEvent = new CommandAsyncEvent(command);
            CommandAsyncEvents.Enqueue(asyncEvent);
            await context.WriteAndFlushAsync(command);
            return await asyncEvent.Task as CommandReply;
        }
    }
}
