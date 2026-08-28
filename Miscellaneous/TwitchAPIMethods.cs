using ooceBot.Authorization;
using ooceBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ooceBot.Miscellaneous
{
    public static class TwitchAPIMethods
    {
        public static async Task<TimeSpan> GetFollowAgeStatsForChatter(string chatterId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"]);
                client.DefaultRequestHeaders.Add("Client-Id", ConfigurationManager.AppSettings["TwitchClientID"]);

                var followAgeUrl = $"https://api.twitch.tv/helix/channels/followers?broadcaster_id={BotVariables.BroadcasterID}&user_id={chatterId}";
                var response = await client.GetStringAsync(followAgeUrl);

                // Parse the incoming response from the GET request
                using var doc = JsonDocument.Parse(response);
                var data = doc.RootElement.GetProperty("data");

                // Check if data contains something of use
                if (data.GetArrayLength() > 0)
                {
                    var followedAt = data[0].GetProperty("followed_at").GetString();

                    return DateTime.Now - DateTime.Parse(followedAt);
                }
                else
                {
                    throw new Exception("Looks like that user isn't a follower.");
                }
            }
        }

        /// <summary>
        /// Calculates the relevant time data for the !followage command
        /// </summary>
        /// <param name="spanData">A TimeSpan value for the length of time spent as a follower</param>
        /// <returns></returns>
        public static FollowAgeData FormatFollowAgeData(TimeSpan spanData)
        {
            FollowAgeData returnValue = new();
            const int DAYS_IN_YEAR = 365;
            const int MONTHS_IN_YEAR = 12;

            // When dividing a double by an int, the int is implicitly converted to a double and the calculation returns a double. Explicitly casting to int at the end drops any decimals
            returnValue.TotalYears = (int)(spanData.TotalDays / DAYS_IN_YEAR);

            // Calculate leftover days from above and use that value to get total months
            int remainingDays = (int)(spanData.TotalDays - (returnValue.TotalYears * DAYS_IN_YEAR));


            returnValue.TotalDays = (int)Math.Truncate(spanData.TotalDays) - (returnValue.TotalYears * DAYS_IN_YEAR);

            return returnValue;
        }
    }
}
