using System;
using Nancy;
using DataKit.Server.Utilities;

namespace DataKit.Server.Modules
{
    public sealed class DataControlModule : NancyModule
    {
        public DataControlModule() : base("/r")
        {
            var listener = DataKitRegistry.Listener;
            Get("/connected", _ =>
            {
                var clients = listener.EnumerateClients();
                return Response.AsJsonNet(clients);
            });

            Post("/createchannel/{deviceId}", args =>
            {
                var deviceId = (string) args.deviceId;
                var channelId = listener.CreateListenerChannel(deviceId);
                if (channelId == null) return HttpStatusCode.BadRequest;
                return channelId;
            });

            Post("/destroychannel/{channelId}", args =>
            {
                listener.DestroyListenerChannel((string) args.channelId);
                return HttpStatusCode.OK;
            });

            Post("/channel/{id}/start", args =>
            {
                try
                {
                    var receiver = listener.GetReceiver((string) args.id);
                    receiver.StartCollection();
                    return HttpStatusCode.OK;
                }
                catch (Exception e)
                {
                    return HttpStatusCode.BadRequest;
                }
            });

            Post("/channel/{id}/stop", args =>
            {
                try
                {
                    var receiver = listener.GetReceiver((string) args.id);
                    receiver.StopCollection();
                    return HttpStatusCode.OK;
                }
                catch (Exception e)
                {
                    return HttpStatusCode.BadRequest;
                }
            });

            Get("/channel/{id}/getdata", args =>
            {
                try
                {
                    var receiver = listener.GetReceiver((string) args.id);
                    return Response.AsJsonNet(receiver.Channel.Data);
                }
                catch (Exception e)
                {
                    return HttpStatusCode.BadRequest;
                }
            });
        }
    }
}