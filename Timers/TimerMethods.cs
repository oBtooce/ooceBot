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
            "Typing !here will take your attendance. If you report in enough times, you get rewards!",
            "Birds are not real (!bird)",
            "Twitch Prime subscriptions are greatly appreciated!",
            "Follow this channel to support local businesses.",
            "ʕ•ᴥ•ʔ",
            new ZalgoString("AHAHAHAHAHAHAHAHAHAHAHA", FuckUpMode.Max, FuckUpPosition.All).ToString(),
            "Type !stats (username) to see someone's chess.com ratings!",
            "Check out my socials: https://linktr.ee/obtooce",
            "Send a challenge (!join) (chess only)",
            "Song requests (!sr song-title-or-yt-link)",
            "Roll the dice (!d6 / !d20 / !d100)!",
            "A list of commands can be found here: !help",
            "Follower emotes: obtoocBri obtoocF obtoocW obtoocNice obtoocOmg",
        };

        public static async Task PostMessageInChat(TwitchClient client, TimeSpan downtime)
        {
            using var timer = new PeriodicTimer(downtime);

            // Make a copy of the messages list so that we can remove as we go
            List<string> messages = new List<string>(MessagesToDisplay);

            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
                var index = random.Next(messages.Count);

                client.SendMessage(BotVariables.ChannelToJoin, messages[index]);

                messages.RemoveAt(index);

                // If we hit the end of the current list, reinitialize it
                if (messages.Count == 0)
                    messages = new List<string>(MessagesToDisplay);
            }
        }
    }
}