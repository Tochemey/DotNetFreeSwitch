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