using DataKit.Server.Listener;

namespace DataKit.Server
{
    /// <summary>
    /// Registry singleton
    /// </summary>
    public class DataKitRegistry
    {
        public static DataKitListener Listener { get; set; }
    }
}