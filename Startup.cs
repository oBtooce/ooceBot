using ooceBot.Authorization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;

namespace ooceBot
{
    public static class Startup
    {
        public static HttpClient SetupNightbotClient()
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["NightbotAPIRequestUri"]!)
            };

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["NightbotOAuthToken"]);

            return client;
        }

        /// <summary>
        /// Sets up the TwitchClient object using information attached to the streamer bot.
        /// </summary>
        /// <returns></returns>
        public static TwitchClient SetupTwitchClient()
        {
            // Set up client
            ConnectionCredentials credentials = new ConnectionCredentials(BotVariables.BotUsername, ConfigurationManager.AppSettings["TwitchBotAccessToken"]);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            var customClient = new WebSocketClient(clientOptions);
            TwitchClient client = new TwitchClient(customClient);
            client.Initialize(credentials);

            return client;
        }

        public static async void UpdateHelixVariables(TwitchAPI api)
        {
            // Get the broadcaster ID for later use
            var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> { BotVariables.ChannelToJoin });
            BotVariables.BroadcasterID = users.Users[0].Id;

            var currentStream = await api.Helix.Streams.GetStreamsAsync(userLogins: new List<string> { BotVariables.ChannelToJoin });

            if (currentStream.Streams.Length > 0)
                BotVariables.StreamStartTime = DateOnly.FromDateTime(currentStream.Streams[0].StartedAt);
            else
                BotVariables.StreamStartTime = DateOnly.FromDateTime(DateTime.UtcNow);

            // Get all currently enabled custom rewards
            var customRewards = await api.Helix.ChannelPoints.GetCustomRewardAsync(BotVariables.BroadcasterID);

            // Add each value to the dictionary
            var redeemData = customRewards.Data.Where(reward => reward.IsEnabled).OrderBy(reward => reward.Cost).ToList();

            // Setting start value to 1 to make it easier for user selection
            for (int i = 0; i < redeemData.Count; i++)
                BotVariables.CustomRewards.Add(i + 1, redeemData[i]);

            await GistManager.UpdateRewardsGist(BotVariables.CustomRewards);
        }
    }
}
