using Nancy;
using DataKit.Server.Utilities;

namespace DataKit.Server.Modules
{
    public class DataControlModule : NancyModule
    {
        public DataControlModule() : base("/r")
        {
            var listener = DataKitRegistry.Listener;
            Get("/connected", _ =>
            {
                var clients = listener.EnumerateClients();
                return Response.AsJsonNet(clients);
            });

            Post("/createchannel/{id}", async args =>
            {
                var id = (string) args.id;
                var channelId = listener.CreateListenerChannel(id);
                return channelId;
            });
        }
    }
}