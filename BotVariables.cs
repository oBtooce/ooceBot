using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ooceBot
{
    public static class BotVariables
    {
        #region Twitch Bot Variables
                
        public static string TwitchOAuthRefreshUri { get; } = "https://id.twitch.tv/oauth2/token";

        // Token that comes from the oBtooce channel (needed for things such as changing stream titles)
        public static string? BroadcasterOAuthToken { get; set; }

        public static string ChannelToJoin { get; } = "obtooce";

        public static string BotUsername { get; } = "oocebot";

        #endregion

        #region WebSocket Variables
        public static Uri WebSocketUri { get; } = new Uri("wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds=30");
        #endregion

        #region OAuth Variables

        public static string ValidationURL { get; } = "https://id.twitch.tv/oauth2/validate";

        #endregion

        #region Email Variable
        public static string Email { get; } = "alex.b.waddell@gmail.com";
        #endregion

        #region Constants
        public static char[] PIECE_NOTATION = new char[6] { 'B', 'K', 'N', 'Q', 'R', ' ' };
        public static char[] FILE_NOTATION = new char[8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        public static int[] RANK_NOTATION = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        #endregion

        #region Application Credentials
        /* Twitch */
        public static string TwitchClientID { get; set; } = "f9t5ykdl3webq5v5q7pfqtre8ib7sx";
        public static string TwitchClientSecret { get; set; } = "nfk9j3tlkzyot58jebfesa6gxeoxbr";
        public static string? TwitchOAuthToken { get; set; }

        /* OAuth */
        public static string TwitchOAuthRefreshToken { get; set; } = "aptgvggr3xyioopcly47m2fuzqmn22gbgjt1yyz0r1v13ykvaz";

        /* OBS Websocket */
        public static string SoundFolder = "C:\\Users\\Alex Waddell\\AppData\\Roaming\\Elgato\\StreamDeck\\Audio";
        public static string OBSWebsocketAddress = "ws://localhost:4455";
        public static string OBSWebsocketPassword = "O1NWPY6OgAgJgvkB";

        /* Nightbot */
        public static string NightbotClientID { get; set; } = "1ff0d546c5a95c6b6e525d7e0421e905";
        public static string NightbotClientSecret { get; set; } = "22ec63730f0ee6340a27fad27c9a48a56eed9409b8a59197bbd0c5e425bb7f30";

        public static string NightbotUriValue = "https://api.nightbot.tv/";

        public static string NightbotApiRequestUriValue = "https://api.nightbot.tv/1/";

        public static string NightbotRedirectUri = "http://localhost:3000";

        public static string NightbotOAuthRefreshUri = "https://api.nightbot.tv/oauth2/token";

        public static string NightbotOAuthRefreshToken { get; set; } = "86c153fc0077497bd92299a42fc7556f500e621b425dbb43bb1a3112ffe3c371";

        public static string? NightbotOAuthToken { get; set; } = "7ae42130aaf0362f25bde2b6930c0f567b7492bcd0c6c2f32341ba5c869f55b6";

        #endregion
    }
}
