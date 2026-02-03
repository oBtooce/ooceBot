using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.PubSub.Models.Responses;

namespace ooceBot.Miscellaneous
{
    public static class VolumeControl
    {
        public static async Task<int> GetNightbotCurrentVolume(HttpClient client)
        {
            int volume = 0;

            var nightbotSongRequests = await client.GetAsync("song_requests");

            nightbotSongRequests.EnsureSuccessStatusCode();

            var json = await nightbotSongRequests.Content.ReadAsStringAsync();

            if (json != string.Empty && json != null)
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("settings", out JsonElement joinedThing) == true)
                    {
                        root.GetProperty("settings").GetProperty("volume").TryGetInt32(out volume);
                    }
                }
            }

            return volume;
        }

        public static async Task<int> IncreaseVolume(HttpClient client, int volume, int change)
        {
            int increasedVolume = volume + change;

            if (increasedVolume > 100)
                increasedVolume = 100;

            var formData = new Dictionary<string, string>
            {
                { "volume", increasedVolume.ToString() }
            };

            HttpContent formContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PutAsync($"{BotVariables.NightbotApiRequestUriValue}/song_requests", formContent);

            if (response.IsSuccessStatusCode)
                return increasedVolume;
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");

                return increasedVolume;
            }
        }

        public static async Task<int> ReduceVolume(HttpClient client, int volume, int change)
        {
            int loweredVolume = volume - change;

            if (loweredVolume < 0)
                loweredVolume = 0;

            var formData = new Dictionary<string, string>
            {
                { "volume", loweredVolume.ToString() }
            };

            HttpContent formContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PutAsync($"{BotVariables.NightbotApiRequestUriValue}/song_requests", formContent);

            if (response.IsSuccessStatusCode)
                return loweredVolume;
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");

                return loweredVolume;
            }
        }
    }
}
