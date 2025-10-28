using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.PubSub.Events;

namespace ooceBot.Commands
{
    public static class GeneralCommandMethods
    {
        public static string ScamFilePath { get; set; } = $"{Directory.GetCurrentDirectory()}/scamFile.txt";

        public static void RedeemChannelPoints(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Console.ReadLine();
        }
    }
}
