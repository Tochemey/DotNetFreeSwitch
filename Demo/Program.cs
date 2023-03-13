using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetFreeSwitch.Commands;
using DotNetFreeSwitch.Common;
using DotNetFreeSwitch.Events;
using DotNetFreeSwitch.Handlers.inbound;
using DotNetFreeSwitch.Handlers.outbound;

namespace Demo
{
   class Program
   {
      static async Task Main(string[] args)
      {
         const string address = "localhost";
         const string password = "ClueCon";
         const int port = 8021;
         const int serverPort = 9090;

         var client = new OutboundSession(address,
             port,
             password);
         await client.ConnectAsync();

         Console.WriteLine("client connected:{0}", client.IsActive());

         Thread.Sleep(3000);

         Console.WriteLine("client Authenticated:{0}",
             client.IsSessionReady());
         var @event = "plain CHANNEL_HANGUP CHANNEL_HANGUP_COMPLETE";
         var subscribed = client.SubscribeAsync(@event).ConfigureAwait(false);


         var commandString = "sofia profile external gwlist up";
         var response = await client.SendApiAsync(new ApiCommand(commandString)).ConfigureAwait(false);
         Console.WriteLine("response:");
         Console.WriteLine("{0}", response.Response);

         commandString = "sofia status";
         response = await client.SendApiAsync(new ApiCommand(commandString)).ConfigureAwait(false);
         Console.WriteLine("response:");
         Console.WriteLine("{0}", response.Response);

         // create an inbound server
         var inboundServer = new InboundServer(serverPort, new DefaultInboundSession());

         // start the server and wait for it to properly start        
         await inboundServer.StartAsync();

         Thread.Sleep(3000);

         Console.WriteLine("Tcp Server started:{0}", inboundServer.Started());

         // let us originate some call
         var callCommand = "{ignore_early_media=false,originate_timeout=120}sofia/internal/1000@127.0.0.1:5060 &socket(192.168.8.169:9090 async)";

         var id = await client.SendBgApiAsync(new BgApiCommand("originate", callCommand));

         Console.WriteLine("command id:{0}", id);
         System.Console.ReadKey();
      }
   }

   public class DefaultInboundSession : InboundSession
   {
      private const string AudioFile = "https://s3.amazonaws.com/plivocloud/Trumpet.mp3";

      protected override Task HandleEvents(Event @event,
          EventType eventType)
      {
         Console.WriteLine(@event);
         return Task.CompletedTask;
      }

      protected override Task PreHandleAsync() { return Task.CompletedTask; }

      protected override async Task HandleAsync() { await PlayAsync(AudioFile); }
   }
}
