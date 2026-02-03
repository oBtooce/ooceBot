using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ooceBot.Authorization
{
    public static class TwitchOAuthManager
    {
        public static async Task SetTwitchOAuthToken()
        {
            HttpClient TwitchApiCallClient = new HttpClient()
            {
                BaseAddress = new Uri(BotVariables.TwitchOAuthRefreshUri),
            };

            // Start out with a newly refreshed token and assign the resulting value to the property in BotVariables for later
            var twitchOAuthToken = await RefreshTwitchOAuthToken(TwitchApiCallClient);
            BotVariables.TwitchOAuthToken = twitchOAuthToken;
        }

        private static async Task<string> RefreshTwitchOAuthToken(HttpClient client)
        {
            // Build the POST content
            var formData = new Dictionary<string, string>
            {
                { "client_id", BotVariables.TwitchClientID },
                { "client_secret", BotVariables.TwitchClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", BotVariables.TwitchOAuthRefreshToken }
            };

            HttpContent requestContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PostAsync(BotVariables.TwitchOAuthRefreshUri, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    return $"oauth:{doc.RootElement.GetProperty("access_token")}";
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");

                return string.Empty;
            }
        }
    }
}
