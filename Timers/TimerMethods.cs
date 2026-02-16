using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Timers
{
    public static class TimerMethods
    {
        private static string _twitchBroadcasterID = string.Empty;
        public static string TwitchBroadcasterID
        {
            get
            {
                if (_twitchBroadcasterID == string.Empty)
                {
                    HttpClient getCallClient = new HttpClient();

                    getCallClient.DefaultRequestHeaders.Authorization =    Add("User-Agent", $"MyChessApp/1.0 ({ConfigurationManager.AppSettings["Email"]})");

                    HttpResponseMessage response = await getCallClient.GetAsync($"https://api.chess.com/pub/player/{username}");
                }
            }
        }
    }
}
