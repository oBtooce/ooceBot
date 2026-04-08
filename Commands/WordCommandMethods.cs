using ooceBot.Functionality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Commands
{
    public static class WordCommandMethods
    {
        public static void F(CommandArgs args)
        {
            string message = $"{BotVariables.obtoocF} {BotVariables.obtoocF} {BotVariables.obtoocF} {BotVariables.obtoocF} {BotVariables.obtoocF}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Lol(CommandArgs args)
        {
            string message = $"Lmao even";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Nice(CommandArgs args)
        {
            string message = $"{BotVariables.obtoocNice} {BotVariables.obtoocNice} {BotVariables.obtoocNice} {BotVariables.obtoocNice} {BotVariables.obtoocNice}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void W(CommandArgs args)
        {
            string message = $"{BotVariables.obtoocW} {BotVariables.obtoocW} {BotVariables.obtoocW} {BotVariables.obtoocW} {BotVariables.obtoocW}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Wow(CommandArgs args)
        {
            string message = "Hey, that's wild";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }
    }
}
