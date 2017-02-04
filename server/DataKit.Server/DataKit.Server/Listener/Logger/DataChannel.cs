using System;

namespace DataKit.Server.Listener.Logger
{
    public class DataChannel
    {
        public string Identifier { get; } = Guid.NewGuid().ToString("N");
    }
}