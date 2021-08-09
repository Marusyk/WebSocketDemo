using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public class Program
    {
        public static async Task Main()
        {

            try
            {
                var client = new ClientWebSocket();
                client.Options.KeepAliveInterval = TimeSpan.FromMinutes(5);
                client.Options.SetRequestHeader("Authorization", "token");
                client.Options.SetRequestHeader("CustomAuthorization", "custom-token");
                await client.ConnectAsync(new Uri("ws://localhost:5000/heartbeats"), CancellationToken.None);
                Console.WriteLine("Connected!");

                var heartbeat = new
                {
                    Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(),
                };
                var json = JsonSerializer.Serialize(heartbeat);
                var bytes = Encoding.UTF8.GetBytes(json);

                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true,
                    CancellationToken.None);

                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message + " " + e.GetType());
                throw;
            }
            Console.WriteLine("Done.");
        }
    }
}
