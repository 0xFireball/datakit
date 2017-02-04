using System.Net;
using System.Net.Sockets;
using DataKit.Server.Listener;

namespace DataKit.Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var port = 5503;
            if (args.Length > 0)
            {
                port = int.Parse(args[0]);
            }
            var listener = new DataKitListener(new TcpListener(IPAddress.Any, port));
            // Start the listener on the async threadpool
            listener.RunAsync();
            
        }
    }
}