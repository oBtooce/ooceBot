using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.Api.Helix;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace ooceBot
{
    public static class CommandMethods
    {
        public static async void AuditChatter(TwitchClient client, OnMessageReceivedArgs args, string username)
        {
            HttpClient getCallClient = new HttpClient();

            getCallClient.DefaultRequestHeaders.Add("User-Agent", "MyChessApp/1.0 (alex.b.waddell@gmail.com)");

            HttpResponseMessage response = await getCallClient.GetAsync($"https://api.chess.com/pub/player/{username}");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                client.SendMessage(args.ChatMessage.Channel, $"{username} doesn't seem to be a valid username. Try again!");
            else
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = doc.RootElement;

                    root.GetProperty("joined").TryGetDouble(out double startDate);
                    string joinDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(startDate).ToShortDateString();

                    client.SendMessage(args.ChatMessage.Channel, $"{username}'s account creation date is: {joinDate}");
                }
            }
        }
        public static async void GetChesscomStats(TwitchClient client, OnMessageReceivedArgs args, string username)
        {
            HttpClient getCallClient = new HttpClient();

            getCallClient.DefaultRequestHeaders.Add("User-Agent", "MyChessApp/1.0 (alex.b.waddell@gmail.com)");

            HttpResponseMessage response = await getCallClient.GetAsync($"https://api.chess.com/pub/player/{username}/stats");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                client.SendMessage(args.ChatMessage.Channel, $"{username} doesn't seem to be a valid username. Try again!");
            else
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = doc.RootElement;

                    string rapidRating = root.GetProperty("chess_rapid").GetProperty("last").GetProperty("rating").ToString();
                    string blitzRating = root.GetProperty("chess_blitz").GetProperty("last").GetProperty("rating").ToString();
                    string bulletRating = root.GetProperty("chess_bullet").GetProperty("last").GetProperty("rating").ToString();
                    
                    client.SendMessage(args.ChatMessage.Channel, $"Rapid: {rapidRating} | Blitz: {blitzRating} | Bullet: {bulletRating}");
                }
            }
        }
    }
}
