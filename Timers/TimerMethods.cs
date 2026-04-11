using ooceBot.Functionality;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using Zalgo;

namespace ooceBot.Timers
{
    public static class TimerMethods
    {
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private static Random random = new Random();

        private static List<string> MessagesToDisplay = new List<string>
        {
            "Remember to file your taxes.",
            "BOO! Gotcha nerd",
            $"Typing !here will take your attendance. If you report in enough times, you get rewards{BotVariables.obtoocBri}",
            $"Twitch Prime subscriptions are greatly appreciated {BotVariables.obtoocBri}",
            "Follow this channel to support local businesses.",
            "ʕ•ᴥ•ʔ",
            new ZalgoString("AHAHAHAHAHAHAHAHAHAHAHA", FuckUpMode.Max, FuckUpPosition.All).ToString(),
            $"Type !stats (username) to see someone's chess.com ratings {BotVariables.obtoocBri}",
            "Check out oBtooce's socials: https://linktr.ee/obtooce",
            "Song requests (!sr song-title-or-yt-link)",
            $"Got a big personality? Type !p to show it off {BotVariables.obtoocBri}",
            "A list of commands can be found here: !help",
            $"Follower emotes: {BotVariables.obtoocBri} {BotVariables.obtoocF} {BotVariables.obtoocW} {BotVariables.obtoocNice} {BotVariables.obtoocOmg}",
            $"Donate 100 bits to receive a COOL PRIZE {BotVariables.obtoocBri}"
        };

        public static async Task PostMessageInChat(TwitchClient client, TimeSpan downtime)
        {
            using var timer = new PeriodicTimer(downtime);

            // Make a copy of the messages list so that we can remove as we go
            List<string> messages = new List<string>(MessagesToDisplay);

            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
                var index = random.Next(messages.Count);

                string message = messages[index];

                client.SendMessage(BotVariables.ChannelToJoin, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);

                messages.RemoveAt(index);

                // If we hit the end of the current list, reinitialize it
                if (messages.Count == 0)
                    messages = new List<string>(MessagesToDisplay);
            }
        }
    }
}