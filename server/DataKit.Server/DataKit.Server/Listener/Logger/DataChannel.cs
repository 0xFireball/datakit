using System;
using System.Collections.Generic;

namespace DataKit.Server.Listener.Logger
{
    public class DataChannel
    {
        public string Identifier { get; } = Guid.NewGuid().ToString("N");
        public List<string> Data { get; } = new List<string>();
    }
}