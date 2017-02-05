using System;
using System.Collections.Generic;

namespace DataKit.Server.Listener.Logger
{
    public class DataChannel
    {
        public string Identifier { get; } = Guid.NewGuid().ToString("N");
        public List<LoggerData> Data { get; } = new List<LoggerData>();
    }
}