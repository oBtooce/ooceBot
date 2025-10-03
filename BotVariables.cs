using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot
{
    public static class BotVariables
    {
        public static string ClientID { get; set; } = "f9t5ykdl3webq5v5q7pfqtre8ib7sx";

        public static string ClientSecret { get; set; } = "nfk9j3tlkzyot58jebfesa6gxeoxbr";
        public static string OAuthToken { get; set; } = "oauth:i3j3u8yd6wc4jkkw2n6juukkddid13";

        public static string OAuthRefreshToken { get; set; } = "0917i441bbgrgo2ndbwjxfjtkzlalrui4newg56qiilofhgvhu";

        public static string ChannelToJoin { get; } = "obtooce";

        public static string BotUsername { get; } = "oocebot";

        public static string RefreshUri { get; } = "https://id.twitch.tv/oauth2/token";
    }
}
