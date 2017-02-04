using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataKit.Server.Listener.Client;

namespace DataKit.Server.Listener
{
    public class DataKitListener
    {
        private readonly TcpListener _hostSocket;
        private readonly ConcurrentBag<ConnectedClient> _clients;

        public DataKitListener(TcpListener hostSocket)
        {
            _hostSocket = hostSocket;
        }

        public async Task RunAsync()
        {
            _hostSocket.Start();
            while (true)
            {
                var client = await _hostSocket.AcceptTcpClientAsync();
                BeginRegistration(client);
            }
        }

        private async Task BeginRegistration(TcpClient client)
        {
            var _clientWriter = new StreamWriter(client.GetStream());
            var _clientReader = new StreamReader(client.GetStream());

            var hello = await _clientReader.ReadLineAsync();
            // TODO: parse hello
            // send ack
            var clientGuid = Guid.NewGuid().ToString("N");
            await _clientWriter.WriteLineAsync($"ACK|{clientGuid}");
            await _clientWriter.FlushAsync();
            // Register the client
            _clients.Add(new ConnectedClient(_clientReader, _clientWriter)
            {
                Uid = clientGuid
            });
        }
    }
}