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

        public Pipelines<LoggerData, bool> DataPipeline { get; } = new Pipelines<LoggerData, bool>();

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
                        var parsedData = new LoggerData(data);
                        Channel.Data.Add(parsedData);
                        // Call data pipelines
                        foreach (var handler in DataPipeline.GetHandlers())
                        {
                            await handler.Invoke(parsedData);
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