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
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocF obtoocF obtoocF obtoocF obtoocF");
        }

        public static void Lol(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"Lmao even");
        }

        public static void Nice(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocNice obtoocNice obtoocNice obtoocNice obtoocNice");
        }

        public static void W(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocW obtoocW obtoocW obtoocW obtoocW");
        }

        public static void Wow(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "Hey, that's wild");
        }
    }
}
