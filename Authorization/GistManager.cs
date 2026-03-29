using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TwitchLib.Api.Helix.Models.ChannelPoints;

namespace ooceBot.Authorization
{
    public static class GistManager
    {
        public static async Task UpdateRewardsGist(Dictionary<int, CustomReward> redeemDictionary)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# obtooce Channel Point Rewards");
            sb.AppendLine();
            sb.AppendLine("| Reward | Cost | Selection |");
            sb.AppendLine("| ------ | ---- | --------- |");

            for (var i = 1;  i <= redeemDictionary.Count; i++)
            {
                var reward = redeemDictionary[i];
                sb.AppendLine($"| {reward.Title} | {reward.Cost:N0} points | {i} |");
            }

            var body = JsonSerializer.Serialize(new
            {
                files = new Dictionary<string, object>
                {
                    ["rewards.md"] = new { content = sb.ToString() }
                }
            });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["GitHubToken"]);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(BotVariables.BotUsername);

            var response = await client.PatchAsync(
                $"https://api.github.com/gists/{ConfigurationManager.AppSettings["GitHubGistId"]}",
                new StringContent(body, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"Gist update failed: {response.StatusCode}");
        }
    }
}
