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
        public static string ClientID { get; set; } = "f9t5ykdl3webq5v5q7pfqtre8ib7sx";

        public static string ClientSecret { get; set; } = "nfk9j3tlkzyot58jebfesa6gxeoxbr";
        public static string? OAuthToken { get; set; }

        public static string OAuthRefreshToken { get; set; } = "aptgvggr3xyioopcly47m2fuzqmn22gbgjt1yyz0r1v13ykvaz";
        
        public static string OAuthRefreshUri { get; } = "https://id.twitch.tv/oauth2/token";

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

        public static string SOUND_FOLDER = "C:\\Users\\Alex Waddell\\AppData\\Roaming\\Elgato\\StreamDeck\\Audio";
        public static string OBS_WEBSOCKET_ADDRESS = "ws://localhost:4455";
        public static string OBS_WEBSOCKET_PASSWORD = "O1NWPY6OgAgJgvkB";
        #endregion
    }
}
