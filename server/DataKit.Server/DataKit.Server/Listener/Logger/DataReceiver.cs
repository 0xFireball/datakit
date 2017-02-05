using System;
using System.Threading.Tasks;
using DataKit.Server.Listener.Client;
using DataKit.Server.Utilities;

namespace DataKit.Server.Listener.Logger
{
    public class DataReceiver
    {
        private readonly ConnectedClient _client;
        public DataChannel Channel { get; } = new DataChannel();
        private bool _collecting = false;

        public Pipelines<string, bool> DataPipeline { get; } = new Pipelines<string, bool>();

        public DataReceiver(ConnectedClient client)
        {
            _client = client;
            RegisterIntoPipeline();
        }

        private async Task RegisterIntoPipeline()
        {
            _client.ReceivePipeline.AddItemToStart(async (data) =>
            {
                if (data.StartsWith(">", StringComparison.Ordinal))
                {
                    if (_collecting)
                    {
                        Channel.Data.Add(data);
                        // Call pipelines
                        foreach (var handler in DataPipeline.GetHandlers())
                        {
                            await handler.Invoke(data);
                        }
                    }
                }
                return false;
            });
        }

        public async Task StartCollection()
        {
            _collecting = true;
            await _client.Output.WriteLineAsync("START");
            await _client.Output.FlushAsync();
        }

        public async Task StopCollection()
        {
            _collecting = false;
            await _client.Output.WriteLineAsync("STOP");
            await _client.Output.FlushAsync();
        }
    }
}