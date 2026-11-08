using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class Streamer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // Compared against BotVariables.StreamStartTime (like Chatter.LastChattedStreamDate) so each stream gets one shoutout,
        // and a bot restart mid-stream doesn't fire a second one
        public DateOnly? LastShoutoutStreamDate { get; set; }
    }
}
