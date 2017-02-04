using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DataKit.Server.Listener.Client;
using DataKit.Server.Listener.Logger;

namespace DataKit.Server.Listener
{
    public class DataKitListener
    {
        private readonly TcpListener _hostSocket;
        private readonly List<ConnectedClient> _clients;
        private List<DataReceiver> _receivers;


        public DataKitListener(TcpListener hostSocket)
        {
            _hostSocket = hostSocket;
        }

        public IEnumerable<ConnectedClient> EnumerateClients()
        {
            return _clients;
        }

        public async Task RunAsync()
        {
            await Task.Run(async () =>
            {
                _hostSocket.Start();
                while (true)
                {
                    var client = await _hostSocket.AcceptTcpClientAsync();
                    BeginRegistration(client);
                }
            });
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

        public string CreateListenerChannel(string deviceId)
        {
            var sensorClient = _clients.FirstOrDefault(x => x.Uid == deviceId);
            if (sensorClient == null) return null;
            var receiver = new DataReceiver(sensorClient);
            var channel = receiver.Channel;
            _receivers.Add(receiver);
            return channel.Identifier;
        }

        public DataReceiver GetReceiver(string channelId) => _receivers.FirstOrDefault(
            x => x.Channel.Identifier == channelId);

        public void DestroyListenerChannel(string channelId)
        {
            var receiver = _receivers.FirstOrDefault(x => x.Channel.Identifier == channelId);
            _receivers.Remove(receiver);
        }
    }
}