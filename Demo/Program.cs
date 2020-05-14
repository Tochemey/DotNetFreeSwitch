using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Commands;
using Core.Common;
using Core.Events;
using Core.Handlers.inbound;
using Core.Handlers.outbound;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;
            const int ServerPort = 10000;

            var client = new OutboundSession(address,
                port,
                password);
            client.ConnectAsync().ConfigureAwait(false);

            Thread.Sleep(1000);

            Console.WriteLine("Connected and Authenticated {0}",
                client.IsSessionReady());
            var @event = "plain CHANNEL_HANGUP CHANNEL_HANGUP_COMPLETE";
            var subscribed = client.SubscribeAsync(@event).ConfigureAwait(false);


            var commandString = "sofia profile external gwlist up";
            var response = client.SendApiAsync(new ApiCommand(commandString)).ConfigureAwait(false);
            Console.WriteLine("Api Response {0}",
                response.GetAwaiter().GetResult().ReplyText);

            var inboundServer = new InboundServer(ServerPort,
                new DefaultInboundSession());
            inboundServer.StartAsync().Wait(500);
            var callCommand =
                "{ignore_early_media=false,originate_timeout=120}sofia/gateway/smsghlocalsip/233247063817 &socket(192.168.74.1:10000 async full)";

            client.SendBgApiAsync(new BgApiCommand("originate",
                callCommand)).Wait(500);

            System.Console.ReadKey();
        }
    }
    
    public class DefaultInboundSession : InboundSession
    {
        private const string AudioFile = "https://s3.amazonaws.com/plivocloud/Trumpet.mp3";

        protected override Task HandleEvents(FsEvent @event,
            EventType eventType)
        {
            Console.WriteLine(@event);
            return Task.CompletedTask;
        }

        protected override Task PreHandleAsync() { return Task.CompletedTask; }

        protected override async Task HandleAsync() { await PlayAsync(AudioFile); }
    }
}
