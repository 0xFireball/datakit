using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace DataKit.Server.Web
{
    public class WebSocketHandler
    {
        private WebSocket _ws;
        public WebSocketHandler(WebSocket websocket) 
        {
            _ws = websocket;
        }

        private async Task<string> ReadLine()
        {
            var data = string.Empty;
            while (true)
            {
                var buf = new byte[1];
                var arraySeg = new ArraySegment<byte>(buf);
                await _ws.ReceiveAsync(arraySeg, CancellationToken.None);
                var c = (char)buf[0];
                if (c == '\n') return data;
                data += c;
            }
        }

        public async Task EventLoop()
        {
            while (_ws.State == WebSocketState.Open)
            {
                var data = await ReadLine();
                if (data.StartsWith(">", StringComparison.Ordinal))
                {
                    var chId = data.Substring(1);
                    var receivers = DataKitRegistry.Listener.EnumerateReceivers();
                    var channel = receivers.FirstOrDefault(x => x.Channel.Identifier == chId);
                    // channel.ReceivePipeline.AddItemToStart()
                }
            }
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