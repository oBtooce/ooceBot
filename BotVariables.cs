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
        #region Twitch Bot Variables
        public static string BroadcasterID { get; set; } = "";

        // Token that comes from the oBtooce channel (needed for things such as changing stream titles)
        public static string? BroadcasterOAuthToken { get; set; }

        public static string ChannelToJoin { get; } = "obtooce";

        public static string BotUsername { get; } = "oocebot";

        public static List<CustomReward> CustomRewards { get; set; } = new List<CustomReward>();


        #endregion

        #region WebSocket Variables
        public static Uri WebSocketUri { get; } = new Uri("wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds=30");
        #endregion

        #region Constants
        public static char[] PIECE_NOTATION = new char[6] { 'B', 'K', 'N', 'Q', 'R', ' ' };
        public static char[] FILE_NOTATION = new char[8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        public static int[] RANK_NOTATION = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static int DEFAULT_BUYIN = 100;
        #endregion

        #region Application Credentials
        public static string? TwitchOAuthToken { get; set; }

        #endregion

        #region Command List Information

        public delegate void Command(CommandArgs args);

        public static Dictionary<string, Command> CommandsList { get; set; } = new Dictionary<string, Command>()
        {
            { "!", CommandMethods.Exclaim },
            { "!addquote", CommandMethods.AddQuote },
            { "!audit", CommandMethods.Audit },
            { "!boner", CommandMethods.Boner },
            { "!buyin", CommandMethods.BuyIn },
            { "!croissant", CommandMethods.Croissant },
            { "!dc", CommandMethods.Discord },
            { "!discord", CommandMethods.Discord },
            { "!emotes", CommandMethods.Emotes },
            { "!finecheddar", CommandMethods.FineCheddar },
            { "!game", CommandMethods.Game },
            { "!groove", CommandMethods.Groove },            
            { "!help", CommandMethods.Help },
            { "!here", CommandMethods.Here },
            { "!jacob", CommandMethods.Jacob },
            { "!lurk", CommandMethods.Lurk },
            { "!play", CommandMethods.Play },
            { "!quote", CommandMethods.Quote },
            { "!randomquote", CommandMethods.RandomQuote },
            { "!rq", CommandMethods.RandomQuote },
            { "!rngmove", CommandMethods.RNGMove },
            { "!salute", CommandMethods.Salute },
            { "!schedule", CommandMethods.Schedule },
            { "!sorrow", CommandMethods.Sorrow },
            { "!spotify", CommandMethods.Spotify },
            { "!stats", CommandMethods.Stats },
            { "!steam", CommandMethods.Steam },
            { "!tarf", CommandMethods.Tarf },
            { "!title", CommandMethods.Title },
            { "!twitter", CommandMethods.Twitter },
            { "!twt", CommandMethods.Twitter },
            { "!vid", CommandMethods.Vid },            
            { "!youtube", CommandMethods.YouTube },
            { "!yt", CommandMethods.YouTube },
            //{ "based", CommandMethods.Based },
            { "f", CommandMethods.F },
            { "nice", CommandMethods.Nice },
            { "w", CommandMethods.W }
        };

        public static Dictionary<string, Command> VideoCommands = new Dictionary<string, Command>()
        {
            { "!lobster", CommandMethods.Lobster },
            { "!who", CommandMethods.WHO },
            { "!wtf", CommandMethods.WTF }
        };

        public static Dictionary<string, string> CommandDictionary { get; set; } = new Dictionary<string, string>()
        {
            { "addquote", "Adds a quote that can be accessed in the future." },
            { "audit", "Checks the account age of a specified chess.com account." },
            { "boner", "Special command dedicated to a member of the community. Try it out!" },
            { "buyin", "Get started with the token system." },
            { "croissant", "Learn all about the world of \"one croissant\"!" },
            { "dc", "Links to the Discord." },
            { "discord", "Links to the Discord." },
            { "emotes", "Displays a list of follower emotes." },
            { "finecheddar", "Learn all about the world of \"fine cheddar\"!" },
            { "groove", "Plays a random sound." },
            { "help", "What are you doing... obtoocOmg" },
            { "here", "Tracks attendance in stream. Builds up to a reward at 10 uses!" },
            { "jacob", "Special command dedicated to a member of the community. Try it out!" },
            { "lobster", "DJ Khaled shows up in this one, bro." },
            { "lurk", "Tells people that you will be lurking for now." },
            { "play", "Try the token system!" },
            { "quote", "Displays a random quote from history." },
            { "randomquote", "Displays a random quote from history." },
            { "rq", "Displays a random quote from history." },
            { "rngmove", "Chooses the move to be played in a blitz game (only usable by mods and above)." },
            { "salute", "Song plays!" },
            { "schedule", "Displays the current stream schedule." },
            { "sorrow", "Song plays!" },
            { "spotify", "Displays a link to oBtooce's Spotify page." },
            { "stats", "Checks rapid, blitz, and bullet statistics for the specified chess.com account." },
            { "steam", "Displays a link to oBtooce's Steam page." },
            { "tarf", "Special command dedicated to a member of the community. Try it out!" },
            { "title", "Sets the title of the stream (available for mods and above)." },
            { "twitter", "Displays a link to oBtooce's Twitter page." },
            { "twt", "Displays a link to oBtooce's Twitter page." },
            { "vid", "Displays a link to oBtooce's latest YouTube video." },
            { "who", "Pete Weber is the man." },
            { "wtf", "Seriously though, what the hell, man?" },
            { "youtube", "Displays a link to oBtooce's YouTube page." },
            { "yt", "Displays a link to oBtooce's YouTube page." }
        };

        public static bool IsAudioOrVideoPlaying { get; set; } = false;

        #endregion
    }
}
