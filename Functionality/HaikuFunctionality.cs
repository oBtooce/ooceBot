using ooceBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Functionality
{
    public static class HaikuFunctionality
    {
        /// <summary>
        /// Creates a string that is in a haiku format. For now, there will be a basic structure of Verb -> Preposition ->
        /// </summary>
        /// <returns></returns>
        public static string GenerateHaiku()
        {
            string haiku = string.Empty;
            Random random = new Random();

            int shortLineLength = 5;
            int longLineLength = 7;

            // Create the string line by line
            for (int i = 0; i < 3; i ++)
            {
                int line = i == 1 ? longLineLength : shortLineLength;

                while (line > 0)
                {
                    // Pull a random word from the word bank
                    var wordList = BotVariables.WordBank.Where(word => word.SyllableCount <= line).ToList();
                    var selectedWord = wordList[random.Next(wordList.Count)];

                    haiku += selectedWord.Word + " ";
                    line -= selectedWord.SyllableCount;
                }

                if (i < 2)
                    haiku += "| ";
            }

            return haiku;
        }
    }
}
