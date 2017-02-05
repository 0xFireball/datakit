﻿using System.IO;
using Newtonsoft.Json;

namespace DataKit.Server.Listener.Client
{
    public class ConnectedClient
    {
        [JsonIgnore]
        public StreamReader Input { get; }

        [JsonIgnore]
        public StreamWriter Output { get; }

        public ConnectedClient(StreamReader inputStream, StreamWriter outputStream)
        {
            Input = inputStream;
            Output = outputStream;
        }

        [JsonProperty("id")]
        public string Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("units")]
        public string Units { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }
    }
}