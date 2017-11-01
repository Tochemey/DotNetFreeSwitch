.NET FreeSwitch Library
========================================

## **Overview**
This library helps interact with the FreeSwitch via its mod_event_socket. For more information about the mod_event_socket refer to [FreeSwitch web site](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket). 
The framework is written using [DotNetty](https://github.com/Azure/DotNetty).
In its current state it can help build IVR applications more quickly. 

## **Features**
The library in its current state can be used to interact with FreeSwitch easily in:
* Inbound mode [Event_Socket_Inbound](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket#mod_event_socket-Inbound)
* Outbound mode [Event_Socket_Outbound](https://wiki.freeswitch.org/wiki/Event_Socket_Outbound)
* One good thing it has is that you can implement your own FreeSwitch message encoder and decoder if you do not want to use the built-in ones

## **License**
[Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt)

## **Installation**
For the meantime there is no Nuget package available for it. Just clone it and build it and you are ready to go.

## **Example**
```c#
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
            string callCommand = "{ignore_early_media=false,originate_timeout=120}sofia/gateway/smsghlocalsip/233289063817 &socket(192.168.74.1:10000 async full)";

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

```