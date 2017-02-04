using System.IO;
using Newtonsoft.Json;

namespace DataKit.Server.Listener.Client
{
    public class ConnectedClient
    {
        private readonly StreamReader _inputStream;
        private readonly StreamWriter _outputStream;

        public ConnectedClient(StreamReader inputStream, StreamWriter outputStream)
        {
            _inputStream = inputStream;
            _outputStream = outputStream;
        }

        [JsonProperty("id")]
        public string Uid { get; set; }
    }
}