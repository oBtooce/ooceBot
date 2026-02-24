using ooceBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;
using static System.Net.WebRequestMethods;

namespace ooceBot
{
    public static class BotVariables
    {
        #region Twitch Bot Variables
        // Token that comes from the oBtooce channel (needed for things such as changing stream titles)
        public static string? BroadcasterOAuthToken { get; set; }

        public static string ChannelToJoin { get; } = "obtooce";

        public static string BotUsername { get; } = "oocebot";

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
            { "!groove", CommandMethods.Groove },
            { "!guts", CommandMethods.Guts },
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
            { "!spotify", CommandMethods.Spotify },
            { "!stats", CommandMethods.Stats },
            { "!steam", CommandMethods.Steam },
            { "!tarf", CommandMethods.Tarf },
            //{ "!title", CommandMethods.Title },
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

        #endregion
    }
}
