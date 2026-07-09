using ooceBot.Enums;
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
        public static List<HaikuWord> WordBank = new List<HaikuWord>
        {
            new HaikuWord("clouds", 1, WordClass.Noun),
            new HaikuWord("dream", 1, WordClass.Noun),
            new HaikuWord("joy", 1, WordClass.Noun),
            new HaikuWord("lake", 1, WordClass.Noun),
            new HaikuWord("land", 1, WordClass.Noun),
            new HaikuWord("plains", 1, WordClass.Noun),
            new HaikuWord("rain", 1, WordClass.Noun),
            new HaikuWord("road", 1, WordClass.Noun),
            new HaikuWord("sun", 1, WordClass.Noun),
            new HaikuWord("tree", 1, WordClass.Noun),
            new HaikuWord("truth", 1, WordClass.Noun),
            new HaikuWord("wind", 1, WordClass.Noun),
            new HaikuWord("mountain", 2, WordClass.Noun),
            new HaikuWord("nature", 2, WordClass.Noun),
            new HaikuWord("river", 2, WordClass.Noun),
            new HaikuWord("ocean", 2, WordClass.Noun),
            new HaikuWord("forest", 2, WordClass.Noun),
            new HaikuWord("insight", 2, WordClass.Noun),
            new HaikuWord("adventure", 3, WordClass.Noun),
            new HaikuWord("clarity", 3, WordClass.Noun),

            new HaikuWord("dream", 1, WordClass.Verb),
            new HaikuWord("feel", 1, WordClass.Verb),
            new HaikuWord("love", 1, WordClass.Verb),
            new HaikuWord("sail", 1, WordClass.Verb),
            new HaikuWord("see", 1, WordClass.Verb),
            new HaikuWord("take", 1, WordClass.Verb),
            new HaikuWord("walk", 1, WordClass.Verb),
            new HaikuWord("enter", 2, WordClass.Verb),
            new HaikuWord("explore", 2, WordClass.Verb),
            new HaikuWord("journey", 2, WordClass.Verb),
            new HaikuWord("traverse", 2, WordClass.Verb),
            new HaikuWord("visit", 2, WordClass.Verb),
            new HaikuWord("wander", 2, WordClass.Verb),

            new HaikuWord("clear", 1, WordClass.Adjective),
            new HaikuWord("cold", 1, WordClass.Adjective),
            new HaikuWord("dry", 1, WordClass.Adjective),
            new HaikuWord("slow", 1, WordClass.Adjective),
            new HaikuWord("smooth", 1, WordClass.Adjective),
            new HaikuWord("true", 1, WordClass.Adjective),
            new HaikuWord("wet", 1, WordClass.Adjective),
            new HaikuWord("gentle", 2, WordClass.Adjective),
            new HaikuWord("lovely", 2, WordClass.Adjective),
            new HaikuWord("quiet", 2, WordClass.Adjective),
            new HaikuWord("serene", 2, WordClass.Adjective),
            new HaikuWord("sunny", 2, WordClass.Adjective),
            new HaikuWord("auspicious", 3, WordClass.Adjective),
            new HaikuWord("picturesque", 3, WordClass.Adjective),
            new HaikuWord("angelic", 3, WordClass.Adjective),

            new HaikuWord("for", 1, WordClass.Preposition),
            new HaikuWord("in", 1, WordClass.Preposition),
            new HaikuWord("of", 1, WordClass.Preposition),
            new HaikuWord("on", 1, WordClass.Preposition),
            new HaikuWord("to", 1, WordClass.Preposition),
            new HaikuWord("above", 2, WordClass.Preposition),
            new HaikuWord("around", 2, WordClass.Preposition),
            new HaikuWord("below", 2, WordClass.Preposition),
            new HaikuWord("over", 2, WordClass.Preposition),
            new HaikuWord("under", 2, WordClass.Preposition),
            new HaikuWord("within", 2, WordClass.Preposition),
            new HaikuWord("without", 2, WordClass.Preposition),

            new HaikuWord("the", 1, WordClass.Article),
            new HaikuWord("a", 1, WordClass.Article),
            new HaikuWord("an", 1, WordClass.Article),
            new HaikuWord("some", 1, WordClass.Article),
            new HaikuWord("few", 1, WordClass.Article),
            new HaikuWord("all", 1, WordClass.Article),
            new HaikuWord("all", 1, WordClass.Article),
            new HaikuWord("many", 2, WordClass.Article),
        };

        public static List<List<(WordClass WordType, int Syllables)>> FiveSyllableSentences = new List<List<(WordClass WordType, int Syllables)>>
        {
            // Starting with articles, e.g. "the strong wind blusters", "a sturdy tree grows", etc.
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1), (WordClass.Verb, 2), },
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1), (WordClass.Verb, 1), },
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 2), (WordClass.Verb, 1), },
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 2), (WordClass.Verb, 1), },
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 2) },
            new() { (WordClass.Article, 1), (WordClass.Adjective, 3), (WordClass.Noun, 1) },
            new() { (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 2) },

            // Starting with nouns, e.g. "light unfolds slowly", "water flows in time", etc.
            new() { (WordClass.Noun, 1), (WordClass.Verb, 2), (WordClass.Preposition, 2) },

            // Starting with prepositions
        };

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
                    var wordList = WordBank.Where(word => word.SyllableCount <= line).ToList();
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
