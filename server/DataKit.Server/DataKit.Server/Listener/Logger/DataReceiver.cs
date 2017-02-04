using System.Threading.Tasks;
using DataKit.Server.Listener.Client;

namespace DataKit.Server.Listener.Logger
{
    public class DataReceiver
    {
        private readonly ConnectedClient _client;
        public DataChannel Channel { get; } = new DataChannel();
        private bool _collecting = false;

        public DataReceiver(ConnectedClient client)
        {
            _client = client;
            StartEventLoop();
        }

        private async Task StartEventLoop()
        {
            await Task.Run(async () =>
            {
                while (_collecting)
                {
                    var data = await _client.Input.ReadLineAsync();
                    Channel.Data.Add(data);
                }
            });
        }

        public void StartCollection()
        {
            _collecting = true;
        }

        public void StopCollection()
        {
            _collecting = false;
        }
    }
}