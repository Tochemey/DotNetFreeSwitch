using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModFreeSwitch.Commands;
using ModFreeSwitch.Events;
using ModFreeSwitch.Handlers.outbound;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Console
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            string address = "192.168.74.128";
            string password = "ClueCon";
            int port = 8021;

            OutboundSession client = new OutboundSession(address, port, password);
            client.ConnectAsync()
                .Wait(1000);

            Thread.Sleep(100);

            _logger.Info("Connected and Authenticated {0}", client.CanSend());
            string @event = "plain ALL";
            bool subscribed = client.SubscribeAsync(@event)
                .Wait(500);

            _logger.Info("subscribed {0}", subscribed);
            //client.OnReceivedUnHandledEvent += (source,
            //    e) => {
            //        EslEvent eslEvent = e.EslEvent;
            //        _logger.Warn("Event Received: {0}", eslEvent.EventName);
            //        return Task.CompletedTask;
            //    };

            string commandString = "sofia profile external gwlist up";
            ApiResponse response = client.SendApiAsync(new ApiCommand(commandString)).Result;
            _logger.Warn("Api Response {0}", response.ReplyText);
            System.Console.ReadKey();
        }
    }
}
