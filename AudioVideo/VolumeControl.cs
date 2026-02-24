using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.PubSub.Models.Responses;

namespace ooceBot.Sounds
{
    public static class VolumeControl
    {
        /// <summary>
        /// Gets the current volume of the Nightbot song request client.
        /// </summary>
        /// <param name="client">The Nightbot client</param>
        /// <returns>The current client volume</returns>
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

        /// <summary>
        /// Increases the volume of the Nightbot song requests client.
        /// </summary>
        /// <param name="client">The Nightbot client</param>
        /// <param name="volume">The current volume</param>
        /// <param name="change">The volume adjustment level</param>
        /// <returns>The increased Nightbot client's volume</returns>
        public static async Task<double> IncreaseVolume(HttpClient client, double volume, double change)
        {
            double increasedVolume = volume + change;

            if (increasedVolume > 100)
                increasedVolume = 100;

            var formData = new Dictionary<string, string>
            {
                { "volume", increasedVolume.ToString() }
            };

            HttpContent formContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PutAsync($"{ConfigurationManager.AppSettings["NightbotAPIRequestUri"]}/song_requests", formContent);

            if (response.IsSuccessStatusCode)
                return increasedVolume;
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");

                return increasedVolume;
            }
        }

        /// <summary>
        /// Reduces the volume of the Nightbot song requests client.
        /// </summary>
        /// <param name="client">The Nightbot client</param>
        /// <param name="volume">The current volume</param>
        /// <param name="change">The volume adjustment level</param>
        /// <returns>The reduced Nightbot client's volume</returns>
        public static async Task<double> ReduceVolume(HttpClient client, double volume, double change)
        {
            double loweredVolume = volume - change;

            if (loweredVolume < 0)
                loweredVolume = 0;

            var formData = new Dictionary<string, string>
            {
                { "volume", loweredVolume.ToString() }
            };

            HttpContent formContent = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await client.PutAsync($"{ConfigurationManager.AppSettings["NightbotAPIRequestUri"]}/song_requests", formContent);

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
