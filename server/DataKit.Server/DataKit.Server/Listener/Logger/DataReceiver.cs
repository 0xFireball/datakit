using DataKit.Server.Listener.Client;

namespace DataKit.Server.Listener.Logger
{
    public class DataReceiver
    {
        public DataChannel Channel { get; } = new DataChannel();

        public DataReceiver(ConnectedClient endpoint)
        {
        }
    }
}