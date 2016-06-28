using System;
using System.Threading;
using System.Threading.Tasks;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Common;
using ModFreeSwitch.Events;
using ModFreeSwitch.Handlers.inbound;
using ModFreeSwitch.Handlers.outbound;
using NLog;

namespace ModFreeSwitch.Console {
    internal class Program {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args) {
            const string address = "192.168.74.128";
            const string password = "ClueCon";
            const int port = 8021;
            const int ServerPort = 10000;

            var client = new OutboundSession(address, port, password);
            client.ConnectAsync()
                .ConfigureAwait(false);

            Thread.Sleep(1000);

            _logger.Info("Connected and Authenticated {0}", client.CanSend());
            var @event = "plain CHANNEL_HANGUP CHANNEL_HANGP_COMPLETE";
            var subscribed = client.SubscribeAsync(@event)
                .ConfigureAwait(false);

 
            var commandString = "sofia profile external gwlist up";
            var response = client.SendApiAsync(new ApiCommand(commandString))
                .ConfigureAwait(false);
            _logger.Warn("Api Response {0}", response.GetAwaiter().GetResult().ReplyText);


            var inboundServer = new InboundServer(ServerPort, new DefaultInboundSession());
            inboundServer.StartAsync().Wait(500);
            string callCommand = "{ignore_early_media=false,originate_timeout=120}sofia/gateway/smsghlocalsip/233247063817 &socket(192.168.74.1:10000 async full)";

            client.SendBgApiAsync(new BgApiCommand("originate", callCommand)).Wait(500);

            System.Console.ReadKey();
        }
    }

    public class DefaultInboundSession : InboundSession {
        private const string AudioFile = "https://s3.amazonaws.com/plivocloud/Trumpet.mp3";
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override Task HandleEvents(EslEvent @event,
            EslEventType eventType) {
            _logger.Debug(@event);
            return Task.CompletedTask;
        }

        public override Task PreHandleAsync() {
            return Task.CompletedTask;
        }

        public override async Task HandleAsync() {
            await PlayAsync(AudioFile);
        }
    }
}