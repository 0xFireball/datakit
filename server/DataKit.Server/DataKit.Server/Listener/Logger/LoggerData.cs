using System;
using Newtonsoft.Json;

namespace DataKit.Server.Listener.Logger
{
    public class LoggerData
    {
        [JsonProperty("data")]
        public float Data { get; }

        [JsonProperty("tag")]
        public string Tag { get; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; }
        public LoggerData(string rawData)        
        {
            var rawDataParts = rawData.Split(' ');t
            var tag = rawDataParts[1];
            var rawTimestamp = rawDataParts[2];
            var rawDataFloat = rawDataParts[3];
            Tag = tag;
            Data = float.Parse(rawDataFloat);
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rawTimestamp));
        }
    }
}