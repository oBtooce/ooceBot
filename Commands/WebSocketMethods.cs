using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.Api.Helix;
using TwitchLib.Communication.Clients;
using TwitchLib.PubSub.Events;

namespace ooceBot.Commands
{
    public static class WebSocketMethods
    {
        public static string ScamFilePath { get; set; } = $"{Directory.GetCurrentDirectory()}/scamFile.txt";

        public static Uri TwitchWebSocketUri { get; set; } = new Uri("wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds=30");

        public static ClientWebSocket WebSocketInstance { get; set; } = new ClientWebSocket();

        public static string? WebSocketSessionID { get; set; }

        // TaskCompletionSource<T> is used for manual completion of Tasks.
        public static TaskCompletionSource<string> SessionReady = new TaskCompletionSource<string>();

        public static async void StartListeningForEvents()
        {
            WebSocketInstance = new ClientWebSocket();

            await WebSocketInstance.ConnectAsync(TwitchWebSocketUri, CancellationToken.None);

            _ = Task.Run(ReceiveMessages);
        }

        private static async Task ReceiveMessages()
        {
            // A buffer is needed for storing incoming data from the ReceiveAsync method call
            var buffer = new byte[4096];

            while (WebSocketInstance.State == WebSocketState.Open)
            {
                var result = await WebSocketInstance.ReceiveAsync(buffer, CancellationToken.None);

                // When the result has been returned, convert it from the buffer into a JSON item (buffer -> string -> JSON)
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                JsonDocument jsonResult = JsonDocument.Parse(message);

                // Work with the JSON object to figure out the message type, then handle accordingly
                var root = jsonResult.RootElement;

                var messageType = root.GetProperty("metadata").GetProperty("message_type").GetString();

                switch (messageType)
                {
                    case "notification":
                        string sessionId = await SessionReady.Task;
                        Console.WriteLine(sessionId);
                        break;
                    case "session_keepalive":
                        break;
                    case "session_reconnect":
                        Uri WebSocketReconnectUri = new Uri(root.GetProperty("payload").GetProperty("session").GetProperty("reconnect_url").GetString());
                        await WebSocketInstance.ConnectAsync(WebSocketReconnectUri, CancellationToken.None);
                        break;
                    case "session_welcome":
                        // Get the session ID that will be used for subscribing to events from the stream
                        WebSocketSessionID = root.GetProperty("payload").GetProperty("session").GetProperty("id").GetString();
                        SessionReady.TrySetResult(WebSocketSessionID);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
