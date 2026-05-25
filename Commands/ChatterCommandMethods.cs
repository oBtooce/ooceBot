using ooceBot.Functionality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Commands
{
    public static class ChatterCommandMethods
    {
        public static void Boner(CommandArgs args)
        {
            string message = $"don't get married";
            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Jacob(CommandArgs args)
        {
            string message = $"Blackjack";
            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Tarf(CommandArgs args)
        {
            string message = $"you gotta be bad, you gotta be bold, you gotta be wiser, you gotta be hard, you gotta be tough, you gotta be stronger, you gotta be cool, you gotta be calm, you gotta stay together, all i know love will save the day - corrected";
            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }
    }
}
