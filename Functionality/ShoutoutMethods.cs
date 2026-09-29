using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ooceBot.Functionality
{
    public static class ShoutoutMethods
    {
        private static readonly TimeSpan ShoutoutCooldown = TimeSpan.FromMinutes(2);

        private const int MaxRetryAttempts = 5;

        private static readonly TimeSpan RetryBackoff = TimeSpan.FromSeconds(30);

        private static readonly HttpClient _client = new HttpClient();

        private static readonly Channel<string> _queue = Channel.CreateUnbounded<string>();

        private static DateTime _lastShoutoutUtc = DateTime.MinValue;

        /// <summary>
        /// Queues up incoming shoutout requests to allow fellow streamers to get their name out there
        /// </summary>
        public static void EnqueueShoutout(string toBroadcasterId) => _queue.Writer.TryWrite(toBroadcasterId);

        /// <summary>
        /// Starts the background worker that drains the shoutout queue one at a time, honoring Twitch's cooldown
        /// between calls. Call once at startup.
        /// </summary>
        public static void StartQueueProcessor() => _ = Task.Run(ProcessQueue);

        private static async Task ProcessQueue()
        {
            await foreach (string toBroadcasterId in _queue.Reader.ReadAllAsync())
            {
                TimeSpan sinceLastShoutout = DateTime.UtcNow - _lastShoutoutUtc;

                if (sinceLastShoutout < ShoutoutCooldown)
                    await Task.Delay(ShoutoutCooldown - sinceLastShoutout);

                await SendWithRetry(toBroadcasterId);
            }
        }

        private static async Task SendWithRetry(string toBroadcasterId, int attempt = 1)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://api.twitch.tv/helix/chat/shoutouts?from_broadcaster_id={BotVariables.BroadcasterID}&to_broadcaster_id={toBroadcasterId}&moderator_id={BotVariables.BroadcasterID}");

            request.Headers.Add("Client-Id", ConfigurationManager.AppSettings["TwitchClientID"]);
            request.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"]}");

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _lastShoutoutUtc = DateTime.UtcNow;
                return;
            }

            // Still in cooldown, most likely because a mod fired /shoutout manually outside our own tracking - back off and retry instead of dropping it
            if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < MaxRetryAttempts)
            {
                await Task.Delay(RetryBackoff);
                await SendWithRetry(toBroadcasterId, attempt + 1);
                return;
            }

            Console.WriteLine($"Shoutout to {toBroadcasterId} failed ({(int)response.StatusCode}) after {attempt} attempt(s): {await response.Content.ReadAsStringAsync()}");
        }
    }
}
