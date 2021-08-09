using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebServer
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status505HttpVersionNotsupported;
                await context.Response.WriteAsync("Only WebSocket requests are allowed at this endpoint.");
                await context.Response.CompleteAsync();
                return;
            }

            Console.WriteLine($"Authorization = {context.Request.Headers["Authorizationn"]}");
            Console.WriteLine($"CustomAuthorization = {context.Request.Headers["CustomAuthorization"]}");

            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            /// ...
        }
    }

    public static class Extensions
    {
        public static IApplicationBuilder MapWebSocket(this IApplicationBuilder app, PathString path)
        {
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(5)
            });
            return app.Map(path, _app => _app.UseMiddleware<WebSocketMiddleware>());
        }
    }
}
