using System;
using System.IO;
using System.Net.Sockets;
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

        public ConnectedClient(StreamReader inputStream, StreamWriter outputStream, TcpClient sock)
        {
            Input = inputStream;
            Output = outputStream;
            Socket = sock;
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