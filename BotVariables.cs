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
    }
}
