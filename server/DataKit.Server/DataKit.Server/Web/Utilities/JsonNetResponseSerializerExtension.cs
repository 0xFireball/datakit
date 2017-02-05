using Nancy;
using Newtonsoft.Json;

namespace DataKit.Server.Utilities
{
    public static class JsonNetResponseSerializerExtension
    {
        public static Response AsJsonNet<T>(this IResponseFormatter formatter, T instance)
        {
            var responseData = JsonConvert.SerializeObject(instance);
            return formatter.AsText(responseData, "application/json");
        }
    }
}