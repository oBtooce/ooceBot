using ooceBot.Commands;
using ooceBot.Enums;
using ooceBot.Models;
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

        // List of IDs that require specific code when redeemed
        public static Dictionary<Guid, string> CustomRewardIDMappings { get; set; } = new Dictionary<Guid, string>
        {
            { new Guid("00939822-8032-416f-bdfc-c635e5351b87"), "This guy stinks" },
            { new Guid("043e21a6-864f-4dee-8c3a-2c72cc03e836"), "Windows start" },
            { new Guid("431d2cbe-79dd-4336-82e8-397290fb934b"), "Fail horn" },
            { new Guid("6dcbe0e2-fc8c-42c9-91f9-3f4234185015"), "Got em" },
            { new Guid("b6d9e8af-da96-43ef-bae8-de038839b8da"), "The game of chess" },
            { new Guid("bce37961-72e0-4a25-8c0e-88e03f8e03d0"), "Haha" },
            { new Guid("c2603c03-25f9-408c-95f1-b3aea2a8b8a8"), "Windows end" },
            { new Guid("cd4493b7-1cd9-4236-b6d2-9581dd9f63ac"), "Vine boom" },
            { new Guid("c762c02c-7d17-4b83-a206-1dd57e631f01"), "Applause" },
            { new Guid("403db7bc-ca3d-4380-8a2d-dd002391423e"), "LOBSTER" },
            { new Guid("54e1e600-5dce-43ad-b686-f10db40a379e"), "WHO" },
            { new Guid("708b1881-d1cc-4963-a376-b6f5560509e4"), "WTF" },
            { new Guid("e92820a8-b024-4b69-be1b-6de94282f357"), "Maggie" },
            { new Guid("ef6e5f85-b31d-44e7-b991-e2f11289b57c"), "Homer" }
        };

        public static DateOnly StreamStartTime { get; set; }

        #endregion

        #region WebSocket Variables
        public static Uri WebSocketUri { get; } = new Uri("wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds=30");
        #endregion

        #region General Variables and Constants
        public static readonly char[] PIECE_NOTATION = new char[6] { 'B', 'K', 'N', 'Q', 'R', ' ' };
        public static readonly char[] FILE_NOTATION = new char[8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        public static readonly int[] RANK_NOTATION = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public const int DEFAULT_BUYIN = 100;

        public const int DEFAULT_NIGHTBOT_VOLUME = 60;

        public const int ATTENDANCE_POINT_VALUE = 2000;

        public static readonly HashSet<string> BANNED_WORDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "nigger", "nigga", "faggot", "fag", "chink", "gook", "retard", "retarded", "kike", "tranny", "troon", "spic"
        };

        public static bool IsYelling = false;

        public static List<string> EmoteWords = new List<string>()
        {
            obtoocBri, obtoocF, obtoocNice, obtoocOmg, obtoocW
        };
        #endregion

        #region Links

        public static string DiscordLink = "https://discord.gg/5RTxTFurGF";

        public static string LatestYTVideo = "https://youtu.be/STmFRwBFvqc";

        public static string SpotifyPage = "https://open.spotify.com/user/obtoose";

        public static string SteamPage = "https://steamcommunity.com/id/obtooce/";

        public static string TwitterPage = "https://twitter.com/oBtuuse";

        public static string YouTubeChannel = "https://www.youtube.com/@obtoocevids";

        #endregion

        #region Application Credentials
        public static string? BotOAuthToken { get; set; }

        #endregion

        #region Command List Information

        public delegate void Command(CommandArgs args);

        public static Dictionary<string, Command> AdminCommands { get; set; } = new Dictionary<string, Command>()
        {
            { "back", AdminCommandMethods.Back },
            { "brb", AdminCommandMethods.BRB },
            { "!game", AdminCommandMethods.Game },
            { "!loud", AdminCommandMethods.Loud },
            { "!rngmove", AdminCommandMethods.RNGMove },
            { "!title", AdminCommandMethods.Title },
        };

        public static Dictionary<string, Command> ChatterCommands { get; set; } = new Dictionary<string, Command>()
        {
            { "!boner", ChatterCommandMethods.Boner },
            { "!jacob", ChatterCommandMethods.Jacob },
            { "!tarf", ChatterCommandMethods.Tarf },
        };

        public static Dictionary<string, Command> CommandsList { get; set; } = new Dictionary<string, Command>()
        {
            { "!addquote", CommandMethods.AddQuote },
            { "!audit", CommandMethods.Audit },
            { "!bird", CommandMethods.Bird },            
            { "!buyin", CommandMethods.BuyIn },
            { "!croissant", CommandMethods.Croissant },
            { "!dap", CommandMethods.Dap },
            { "!dc", CommandMethods.Discord },
            { "!discord", CommandMethods.Discord },
            { "!emotes", CommandMethods.Emotes },
            { "!event", CommandMethods.Event },
            { "!finecheddar", CommandMethods.FineCheddar },
            { "!followage", CommandMethods.FollowAge },
            { "!groove", CommandMethods.Groove },
            { "!haiku", CommandMethods.Haiku },
            { "!help", CommandMethods.Help },
            { "!here", CommandMethods.Here },
            { "!hug", CommandMethods.Dap },            
            { "!lurk", CommandMethods.Lurk },
            { "!p", CommandMethods.Personality },
            { "!play", CommandMethods.Play },
            { "!points", CommandMethods.Points },
            { "!present", CommandMethods.Here },
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
            { "!tokens", CommandMethods.Tokens },
            { "!top10", CommandMethods.TopPlayers },
            { "!twitter", CommandMethods.Twitter },
            { "!twt", CommandMethods.Twitter },
            { "!vid", CommandMethods.Vid },
            { "!yay", CommandMethods.Yay },
            { "!youtube", CommandMethods.YouTube },
            { "!yt", CommandMethods.YouTube }
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
