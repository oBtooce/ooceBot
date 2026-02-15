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
    }
}
