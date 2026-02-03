using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ooceBot.Authorization
{
    public static class NighbotOAuthManager
    {
        public static async Task SetNightbotOAuthToken()
        {
            HttpClient NightbotApiCallClient = new HttpClient()
            {
                BaseAddress = new Uri(BotVariables.NightbotUriValue)
            };

            var nightbotOAuthToken = await RefreshNightbotOAuthToken(NightbotApiCallClient);
            BotVariables.NightbotOAuthToken = nightbotOAuthToken;
        }

        public static async Task<string> RefreshNightbotOAuthToken(HttpClient client)
        {
            // Build the POST content
            var formData = new Dictionary<string, string>
            {
                { "client_id", BotVariables.NightbotClientID },
                { "client_secret", BotVariables.NightbotClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", BotVariables.NightbotOAuthRefreshToken }
            };

            HttpContent requestContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PostAsync(BotVariables.NightbotOAuthRefreshUri, requestContent);

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
