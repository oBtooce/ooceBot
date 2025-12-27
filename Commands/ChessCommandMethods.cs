using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.Api.Helix;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace ooceBot.Commands
{
    public static class ChessCommandMethods
    {
        public static async void AuditChatter(TwitchClient client, OnMessageReceivedArgs args, string username)
        {
            HttpClient getCallClient = new HttpClient();

            getCallClient.DefaultRequestHeaders.Add("User-Agent", $"MyChessApp/1.0 ({BotVariables.Email})");

            HttpResponseMessage response = await getCallClient.GetAsync($"https://api.chess.com/pub/player/{username}");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                client.SendMessage(args.ChatMessage.Channel, $"{username} doesn't seem to be a valid username. Try again!");
            else
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                if (jsonString != string.Empty && jsonString != null)
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        JsonElement root = doc.RootElement;

                        // Try to get the joined property and handle the error if it does not exist
                        var property = root.TryGetProperty("joined", out JsonElement joinedThing);

                        if (property == false)
                        {
                            client.SendMessage(args.ChatMessage.Channel, $"'{username}' does not exist in the chesscom database.");
                            return;
                        }

                        double startDate = joinedThing.GetDouble();
                        string joinDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(startDate).ToShortDateString();

                        client.SendMessage(args.ChatMessage.Channel, $"{username}'s account creation date is: {joinDate}");
                    }
                }
                else
                {
                    client.SendMessage(args.ChatMessage.Channel, $"An account with the name of '{username}' does not exist.");
                }
            }
        }
        public static async void GetChesscomStats(TwitchClient client, OnMessageReceivedArgs args, string username)
        {
            HttpClient getCallClient = new HttpClient();

            getCallClient.DefaultRequestHeaders.Add("User-Agent", $"MyChessApp/1.0 ({BotVariables.Email})");

            HttpResponseMessage response = await getCallClient.GetAsync($"https://api.chess.com/pub/player/{username}/stats");

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                client.SendMessage(args.ChatMessage.Channel, $"{username} doesn't seem to be a valid username. Try again!");
            else
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                if (jsonString != string.Empty && jsonString != null)
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        JsonElement root = doc.RootElement;

                        Dictionary<string, string> stats = new Dictionary<string, string>();

                        // Variables for rapid, blitz and bullet
                        string rapidRating, blitzRating, bulletRating;
                        rapidRating = blitzRating = bulletRating = string.Empty;

                        // Null checks on each time control so that things don't break if someone hasn't played a particular one
                        if (root.TryGetProperty("chess_rapid", out JsonElement rapid) == true)
                            stats.Add("Rapid", root.GetProperty("chess_rapid").GetProperty("last").GetProperty("rating").ToString());

                        if (root.TryGetProperty("chess_blitz", out JsonElement blitz) == true)
                            stats.Add("Blitz", root.GetProperty("chess_blitz").GetProperty("last").GetProperty("rating").ToString());

                        if (root.TryGetProperty("chess_blitz", out JsonElement bullet) == true)
                            stats.Add("Bullet", root.GetProperty("chess_bullet").GetProperty("last").GetProperty("rating").ToString());

                        // Make sure that there is something to return here
                        if (stats.Count == 0)
                        {
                            client.SendMessage(args.ChatMessage.Channel, $"An account for {username} exists, but no rapid, blitz or bullet games have been played.");
                            return;
                        }

                        // Create the output string based on the available time control data
                        string outputMessage = $"Stats for {username} (Chess.com) -- ";

                        foreach (var stat in stats)
                        {
                            if (outputMessage.EndsWith("-- "))
                                outputMessage += $"{stat.Key}: {stat.Value}";
                            else
                                outputMessage += $" | {stat.Key}: {stat.Value}";
                        }

                        client.SendMessage(args.ChatMessage.Channel, outputMessage);
                    }
                }
                else
                {
                    client.SendMessage(args.ChatMessage.Channel, $"An account with the name of '{username}' does not exist.");
                }
            }
        }
    }
}
