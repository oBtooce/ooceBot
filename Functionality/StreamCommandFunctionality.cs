using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using static System.Net.Mime.MediaTypeNames;

namespace ooceBot.Functionality
{
    public static class StreamCommandFunctionality
    {
        /// <summary>
        /// MAKE THE CHAT MESSAGE LOUD!
        /// </summary>
        /// <param name="input">A CHAT MESSAGE THAT IS TOO QUIET</param>
        /// <returns></returns>
        public static string MakeItLoud(string input)
        {
            string regexPattern = "https?:\\/\\/(www\\\\.)?[-a-zA-Z0-9@:%._\\\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\\\+.~#?&//=]*)";
            Regex regex = new Regex(regexPattern);

            bool containsEmoteWord = BotVariables.EmoteWords.Any(w => input.Contains(w));

            // If no instances of the regex or emote list are detected, upper the whole thing, otherwise split and work through it
            if (!regex.IsMatch(input) && !containsEmoteWord)
                return input.ToUpper();
            else
            {
                StringBuilder message = new StringBuilder();

                string[] stringPieces = input.Split(' ');

                for (int i = 0; i < stringPieces.Length; i++)
                {
                    string piece = stringPieces[i];

                    // If any matches are made with regex/emotes, output normal stuff
                    if (regex.IsMatch(piece) || BotVariables.EmoteWords.Contains(piece) || !BotVariables.IsYelling)
                        message.Append(piece);
                    else
                        message.Append(piece.ToUpper());

                    // Add a space where needed
                    if (i != stringPieces.Length - 1)
                        message.Append(' ');
                }

                return message.ToString();
            }            
        }
    }
}
