using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DataKit.Server.Web
{
    public class WebSocketHandler
    {
        private WebSocket _ws;
        public WebSocketHandler(WebSocket websocket) 
        {
            _ws = websocket;
        }

        public async Task EventLoop()
        {
            
        }

        public static async Task AcceptWebSocketClients(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var ws = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebSocketHandler(ws);
            await h.EventLoop();
        }
    }
}