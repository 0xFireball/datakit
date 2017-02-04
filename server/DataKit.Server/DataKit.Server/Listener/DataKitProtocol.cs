namespace DataKit.Server.Listener
{
    public class DataKitProtocol
    {
        // Packet format:
        // 1: ushort - message type
        // 2: int - message length (in bytes)
        public enum MessageType
        {
            Hello = 0x0001
        }
    }
}