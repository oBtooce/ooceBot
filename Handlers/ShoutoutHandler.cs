using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace ooceBot.Handlers
{
    public class ShoutoutHandler
    {
        private readonly TwitchClient _client;
        private readonly TwitchAPI _api;

        public ShoutoutHandler(TwitchClient client, TwitchAPI api)
        {
            _client = client;
            _api = api;
        }

        public async void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var msg = e.ChatMessage;
            var parts = msg.Message.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var cmd = parts[0].ToLowerInvariant();
            if (cmd != "!so" && cmd != "!shoutout") return;

            // Only mods and the broadcaster can use it
            if (!msg.IsModerator && !msg.IsBroadcaster) return;

            if (parts.Length < 2)
            {
                _client.SendMessage(msg.Channel, "Usage: !so <username>");
                return;
            }

            var targetLogin = parts[1].TrimStart('@').ToLowerInvariant();

            try
            {
                // 1. Look up the target's user ID
                var users = await _api.Helix.Users.GetUsersAsync(logins: new List<string> { targetLogin });
                var target = users.Users.FirstOrDefault();
                if (target == null)
                {
                    _client.SendMessage(msg.Channel, $"Couldn't find a user named {targetLogin}.");
                    return;
                }

                // 2. Get their last category for the chat message
                var info = await _api.Helix.Channels.GetChannelInformationAsync(target.Id);
                var game = info.Data.FirstOrDefault()?.GameName;

                // 3. Chat message shoutout
                var text = $"Go show some love to {target.DisplayName} at https://twitch.tv/{target.Login}";
                if (!string.IsNullOrWhiteSpace(game)) text += $" - they were last playing {game}!";
                _client.SendMessage(msg.Channel, text);

                // 4. Official Twitch shoutout (follow card)
                await SendShoutoutAsync(target.Id);
            }
            catch (Exception ex)
            {
                // Common causes: stream offline, cooldown (429), missing scope (401), not a mod (403)
                Console.WriteLine($"Shoutout failed: {ex.Message}");
            }
        }

        private static readonly HttpClient Http = new HttpClient();

        private async Task SendShoutoutAsync(string toBroadcasterId)
        {
            var url = $"https://api.twitch.tv/helix/chat/shoutouts?from_broadcaster_id={BotVariables.BroadcasterID}&to_broadcaster_id={toBroadcasterId}&moderator_id={BotVariables.BroadcasterID}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("Client-Id", _api.Settings.ClientId);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["TwitchBotAccessToken"]);

            var res = await Http.SendAsync(req);
            if (!res.IsSuccessStatusCode)   // success is 204 No Content
            {
                var body = await res.Content.ReadAsStringAsync();
                throw new Exception($"Shoutout failed ({(int)res.StatusCode}): {body}");
            }
        }
    }
}
