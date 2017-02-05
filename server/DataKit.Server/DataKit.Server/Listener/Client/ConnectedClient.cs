using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataKit.Server.Utilities;
using Newtonsoft.Json;

namespace DataKit.Server.Listener.Client
{
    public class ConnectedClient
    {
        [JsonIgnore]
        public StreamReader Input { get; }

        [JsonIgnore]
        public StreamWriter Output { get; }

        [JsonIgnore]
        public TcpClient Socket { get; }

        [JsonIgnore]
        public Pipelines<string, bool> ReceivePipeline { get; } = new Pipelines<string, bool>();

        public ConnectedClient(StreamReader inputStream, StreamWriter outputStream, TcpClient sock)
        {
            Input = inputStream;
            Output = outputStream;
            Socket = sock;
            RegisterPipelineHooks();
            StartEventLoop();
        }

        private void RegisterPipelineHooks()
        {
            ReceivePipeline.AddItemToStart(async (data) =>
            {
                if (data == "$P")
                {
                    LastHeartbeat = DateTime.Now;
                }
            });
        }

        private async Task StartEventLoop()
        {
            await Task.Run(async () =>
            {
                var data = await Input.ReadLineAsync();
                data = data.Trim();
                // Call pipelines
                foreach (var handler in ReceivePipeline.GetHandlers())
                {
                    await handler.Invoke(data);
                }
            });
        }

        [JsonProperty("id")]
        public string Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("lastHeartbeat")]
        public DateTime LastHeartbeat { get; set; }
    }
}