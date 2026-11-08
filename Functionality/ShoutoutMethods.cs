using ooceBot.Models;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ooceBot.Functionality
{
    public static class ShoutoutMethods
    {
        // Twitch only allows one shoutout every 2 minutes per channel
        private static readonly TimeSpan ShoutoutCooldown = TimeSpan.FromMinutes(2);

        private const int MaxRetryAttempts = 5;

        private static readonly TimeSpan RetryBackoff = TimeSpan.FromSeconds(30);

        // After a failed shoutout (e.g. stream offline), wait this long before letting a new message from that streamer re-queue it
        private static readonly TimeSpan RequeueDelay = TimeSpan.FromMinutes(1);

        private static readonly HttpClient _client = new HttpClient();

        private static readonly Channel<string> _queue = Channel.CreateUnbounded<string>();

        // Streamer IDs currently waiting in (or being processed by) the queue, so repeated messages don't stack duplicates
        private static readonly ConcurrentDictionary<string, byte> _pending = new ConcurrentDictionary<string, byte>();

        // Streamer IDs whose last attempt failed, mapped to the earliest time they can be queued again
        private static readonly ConcurrentDictionary<string, DateTime> _retryAfterUtc = new ConcurrentDictionary<string, DateTime>();

        // Streamer IDs shouted out since the bot started. The chat thread's shared DbContext may hold a stale LastShoutoutStreamDate,
        // and it isn't thread-safe to update from here, so this covers the current run while the DB covers restarts
        private static readonly ConcurrentDictionary<string, byte> _shoutedOutThisRun = new ConcurrentDictionary<string, byte>();

        private static DateTime _lastShoutoutUtc = DateTime.MinValue;

        /// <summary>
        /// Queues up a Twitch shoutout for a recognized streamer if they haven't received one this stream. Safe to call on every message.
        /// </summary>
        public static void TryEnqueueShoutout(Streamer streamer)
        {
            if (streamer.LastShoutoutStreamDate == BotVariables.StreamStartTime || _shoutedOutThisRun.ContainsKey(streamer.Id))
                return;

            if (_retryAfterUtc.TryGetValue(streamer.Id, out DateTime retryAfter) && DateTime.UtcNow < retryAfter)
                return;

            if (_pending.TryAdd(streamer.Id, 0))
                _queue.Writer.TryWrite(streamer.Id);
        }

        /// <summary>
        /// Starts the background worker that drains the shoutout queue one at a time, honoring Twitch's cooldown
        /// between calls. Call once at startup.
        /// </summary>
        public static void StartQueueProcessor() => _ = Task.Run(ProcessQueue);

        private static async Task ProcessQueue()
        {
            await foreach (string toBroadcasterId in _queue.Reader.ReadAllAsync())
            {
                try
                {
                    TimeSpan sinceLastShoutout = DateTime.UtcNow - _lastShoutoutUtc;

                    if (sinceLastShoutout < ShoutoutCooldown)
                        await Task.Delay(ShoutoutCooldown - sinceLastShoutout);

                    if (await SendWithRetry(toBroadcasterId))
                    {
                        _retryAfterUtc.TryRemove(toBroadcasterId, out _);
                        _shoutedOutThisRun[toBroadcasterId] = 0;
                        MarkShoutedOut(toBroadcasterId);
                    }
                    else
                        _retryAfterUtc[toBroadcasterId] = DateTime.UtcNow + RequeueDelay;
                }
                catch (Exception ex)
                {
                    _retryAfterUtc[toBroadcasterId] = DateTime.UtcNow + RequeueDelay;
                    Console.WriteLine($"Shoutout to {toBroadcasterId} threw an exception: {ex.Message}");
                }
                finally
                {
                    _pending.TryRemove(toBroadcasterId, out _);
                }
            }
        }

        /// <summary>
        /// Records that the streamer got their shoutout for the current stream. Uses its own context since this runs off the chat thread.
        /// </summary>
        private static void MarkShoutedOut(string streamerId)
        {
            using var context = new TwitchBotContext();
            Streamer? streamer = context.Streamers.Find(streamerId);

            if (streamer == null)
                return;

            streamer.LastShoutoutStreamDate = BotVariables.StreamStartTime;
            context.SaveChanges();

        }

        /// <returns>True if Twitch accepted the shoutout</returns>
        private static async Task<bool> SendWithRetry(string toBroadcasterId)
        {
            for (int attempt = 1; ; attempt++)
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://api.twitch.tv/helix/chat/shoutouts?from_broadcaster_id={BotVariables.BroadcasterID}&to_broadcaster_id={toBroadcasterId}&moderator_id={BotVariables.BroadcasterID}");

                // The token must belong to the moderator_id user (the broadcaster) and include the moderator:manage:shoutouts scope
                request.Headers.Add("Client-Id", ConfigurationManager.AppSettings["TwitchClientID"]);
                request.Headers.Add("Authorization", $"Bearer {ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"]}");

                using var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)   // 204 No Content
                {
                    _lastShoutoutUtc = DateTime.UtcNow;
                    Console.WriteLine($"Shoutout sent to {toBroadcasterId}.");
                    return true;
                }

                // Still in cooldown (global 2 min, or 60 min per target), possibly from a manual /shoutout - back off and retry
                if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < MaxRetryAttempts)
                {
                    await Task.Delay(RetryBackoff);
                    continue;
                }

                // 400 usually means the stream is offline; the streamer's next message will re-queue it after RequeueDelay
                Console.WriteLine($"Shoutout to {toBroadcasterId} failed ({(int)response.StatusCode}) after {attempt} attempt(s): {await response.Content.ReadAsStringAsync()}");
                return false;
            }
        }
    }
}
