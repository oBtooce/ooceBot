using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
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
                // ConnectAsync fires back chunks of JSON instead of the whole thing, so we need to handle everything until the end of the message is found
                using (var memoryStream = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await WebSocketInstance.ReceiveAsync(buffer, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine("WebSocket closed by server.");
                            await WebSocketInstance.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            return;
                        }

                        memoryStream.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    string message = Encoding.UTF8.GetString(memoryStream.ToArray());
                    HandleMessage(message);
                }
            }
        }

        private static async void HandleMessage(string message)
        {
            // When the result has been returned, convert it from the buffer into a JSON item (buffer -> string -> JSON)
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
                    Console.WriteLine("Keeping alive");
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
