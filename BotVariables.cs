using ooceBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using static Program;
using static System.Net.WebRequestMethods;

namespace ooceBot
{
    public static class BotVariables
    {
        #region Twitch Emote Variables

        public static string obtoocBri { get; } = "obtoocBri";
        public static string obtoocF { get; } = "obtoocF";
        public static string obtoocOmg { get; } = "obtoocOmg";
        public static string obtoocNice { get; } = "obtoocNice";
        public static string obtoocW { get; } = "obtoocW";

        #endregion
        #region Twitch Bot Variables
        public static string BroadcasterID { get; set; } = "";

        // Token that comes from the oBtooce channel (needed for things such as changing stream titles)
        public static string? BroadcasterOAuthToken { get; set; }

        public static string ChannelToJoin { get; } = "obtooce";

        public static string BotUsername { get; } = "oocebot";

        public static Dictionary<int, CustomReward> CustomRewards { get; set; } = new Dictionary<int, CustomReward>();

        public static string StreamStartTime { get; set; } = string.Empty;

        #endregion

        #region WebSocket Variables
        public static Uri WebSocketUri { get; } = new Uri("wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds=30");
        #endregion

        #region Constants
        public static char[] PIECE_NOTATION = new char[6] { 'B', 'K', 'N', 'Q', 'R', ' ' };
        public static char[] FILE_NOTATION = new char[8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        public static int[] RANK_NOTATION = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static int DEFAULT_BUYIN = 100;

        public static int DEFAULT_NIGHTBOT_VOLUME = 60;

        public static int ATTENDANCE_POINT_VALUE = 2000;

        public static HashSet<string> BANNED_WORDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "nigger", "nigga", "faggot", "fag", "chink", "gook", "retard", "retarded", "kike", "tranny", "troon", "spic"
        };
        #endregion

        #region Application Credentials
        public static string? TwitchOAuthToken { get; set; }

        #endregion

        #region Command List Information

        public delegate void Command(CommandArgs args);

        public static Dictionary<string, Command> AdminCommands { get; set; } = new Dictionary<string, Command>()
        {
            { "back", AdminCommandMethods.Back },
            { "brb", AdminCommandMethods.BRB },
            { "!game", AdminCommandMethods.Game },
            { "!rngmove", AdminCommandMethods.RNGMove },
            { "!title", AdminCommandMethods.Title },
        };

        public static Dictionary<string, Command> CommandsList { get; set; } = new Dictionary<string, Command>()
        {            
            { "!addquote", CommandMethods.AddQuote },
            { "!audit", CommandMethods.Audit },
            { "!bird", CommandMethods.Bird },
            { "!boner", CommandMethods.Boner },
            { "!buyin", CommandMethods.BuyIn },
            { "!croissant", CommandMethods.Croissant },
            { "!dc", CommandMethods.Discord },
            { "!discord", CommandMethods.Discord },
            { "!emotes", CommandMethods.Emotes },
            { "!finecheddar", CommandMethods.FineCheddar },
            { "!groove", CommandMethods.Groove },            
            { "!help", CommandMethods.Help },
            { "!here", CommandMethods.Here },
            { "!jacob", CommandMethods.Jacob },
            { "!lurk", CommandMethods.Lurk },
            { "!play", CommandMethods.Play },
            { "!quote", CommandMethods.Quote },
            { "!randomquote", CommandMethods.RandomQuote },
            { "!rq", CommandMethods.RandomQuote },
            { "!redeem", CommandMethods.Redeem },
            { "!rewards", CommandMethods.Rewards },
            { "!rule", CommandMethods.Rule },
            { "!salute", CommandMethods.Salute },
            { "!schedule", CommandMethods.Schedule },
            { "!sorrow", CommandMethods.Sorrow },
            { "!spotify", CommandMethods.Spotify },
            { "!stats", CommandMethods.Stats },
            { "!steam", CommandMethods.Steam },
            { "!store", CommandMethods.Store },
            { "!tarf", CommandMethods.Tarf },
            { "!tokens", CommandMethods.Tokens },
            { "!top10", CommandMethods.TopPlayers },
            { "!twitter", CommandMethods.Twitter },
            { "!twt", CommandMethods.Twitter },
            { "!vid", CommandMethods.Vid },
            { "!youtube", CommandMethods.YouTube },
            { "!yt", CommandMethods.YouTube }
        };

        public static Dictionary<string, Command> VideoCommands = new Dictionary<string, Command>()
        {
            { "!lobster", VideoCommandMethods.Lobster },
            { "!who", VideoCommandMethods.WHO },
            { "!wtf", VideoCommandMethods.WTF }
        };

        public static Dictionary<string, Command> WordCommands = new Dictionary<string, Command>()
        {
            { "!", CommandMethods.Exclaim },
            { "f", WordCommandMethods.F },
            { "lol", WordCommandMethods.Lol },
            { "nice", WordCommandMethods.Nice },
            { "w", WordCommandMethods.W },
            { "wow", WordCommandMethods.Wow }
        };

        public static Dictionary<string, string> CommandDictionary { get; set; } = new Dictionary<string, string>()
        {
            { "addquote", "Adds a quote that can be accessed in the future." },
            { "audit", "Checks the account age of a specified chess.com account." },
            { "bird", "Shows how many times the Bird Opening has been played on stream." },
            { "boner", "Special command dedicated to a member of the community. Try it out!" },
            { "buyin", "Get started with the arcade token system." },
            { "croissant", "Learn all about the world of \"one croissant\"!" },
            { "dc", "Links to the community Discord." },
            { "discord", "Links to the community Discord." },
            { "emotes", "Displays a list of follower emotes." },
            { "finecheddar", "Learn all about the world of \"fine cheddar\"!" },
            { "groove", "Plays a random sound." },
            { "help", "What are you doing... obtoocOmg" },
            { "here", "Tracks attendance in stream. Builds up to a reward at 10 uses!" },
            { "jacob", "Special command dedicated to a member of the community. Try it out!" },
            { "lobster", "DJ Khaled shows up in this one, bro. (VIP/sub video)" },
            { "lurk", "Tells people that you will be lurking for now." },
            { "play", "Try the token system!" },
            { "quote", "Displays a specified quote from history." },
            { "randomquote", "Displays a random quote from history." },
            { "rq", "Displays a random quote from history." },
            { "rule", "Describes the process for creating a \"royal decree\"." },
            { "salute", "Song plays!" },
            { "schedule", "Displays the current stream schedule." },
            { "sorrow", "Song plays!" },
            { "spotify", "Displays a link to oBtooce's Spotify page." },
            { "stats", "Checks rapid, blitz, and bullet statistics for the specified chess.com account." },
            { "steam", "Displays a link to oBtooce's Steam page." },
            { "tarf", "Special command dedicated to a member of the community. Try it out!" },
            { "tokens", "Displays your current token total from arcade games." },
            { "top10", "Displays the top 10 chatters in terms of total tokens." },
            { "twitter", "Displays a link to oBtooce's Twitter page." },
            { "twt", "Displays a link to oBtooce's Twitter page." },
            { "vid", "Displays a link to oBtooce's latest YouTube video." },
            { "who", "Pete Weber is the man. (VIP/sub video)" },
            { "wtf", "Seriously though, what the hell, man? (VIP/sub video)" },
            { "youtube", "Displays a link to oBtooce's YouTube page." },
            { "yt", "Displays a link to oBtooce's YouTube page." }
        };

        public static bool IsAudioOrVideoPlaying { get; set; } = false;

        #endregion
    }
}
