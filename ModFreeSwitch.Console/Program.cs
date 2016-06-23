using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModFreeSwitch.Handlers.outbound;
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

            EslClient client = new EslClient(address, port, password);
            client.ConnectAsync()
                .Wait(1000);

            _logger.Info("Connected and Authenticated {0}", client.CanSend());
            System.Console.ReadKey();
        }
    }
}
