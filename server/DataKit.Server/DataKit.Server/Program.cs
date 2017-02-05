using System.IO;
using System.Net;
using System.Net.Sockets;
using DataKit.Server.Listener;
using DataKit.Server.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DataKit.Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Start datakit listener
            StartDKListener(args);
            // Start OWIN/Kestrel webserver
            var appDirectory = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(appDirectory)
                .AddCommandLine(args)
                .AddJsonFile(Path.Combine(appDirectory, "hosting.json"), true)
                .Build();


            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(appDirectory)
                .UseStartup<WebStartup>()
                .Build();
            host.Run();
        }

        private static void StartDKListener(string[] args)
        {
            var port = 5503;
            if (args.Length > 0)
            {
                port = int.Parse(args[0]);
            }
            var listener = new DataKitListener(new TcpListener(IPAddress.Any, port));
            DataKitRegistry.Listener = listener;
                // Start the listener on the async threa    dpool
            listener.RunAsync();
        }
    }
}