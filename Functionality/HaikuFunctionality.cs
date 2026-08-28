using ooceBot.Enums;
using ooceBot.Models;
using System;
using System.Collections;
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
            new HaikuWord("leaf", 1, WordClass.Noun),
            new HaikuWord("mood", 1, WordClass.Noun),
            new HaikuWord("plains", 1, WordClass.Noun),
            new HaikuWord("rain", 1, WordClass.Noun),
            new HaikuWord("road", 1, WordClass.Noun),
            new HaikuWord("sun", 1, WordClass.Noun),
            new HaikuWord("tree", 1, WordClass.Noun),
            new HaikuWord("truth", 1, WordClass.Noun),
            new HaikuWord("wind", 1, WordClass.Noun),
            new HaikuWord("zen", 1, WordClass.Noun),
            new HaikuWord("allure", 2, WordClass.Noun),
            new HaikuWord("castle", 2, WordClass.Noun),
            new HaikuWord("desire", 2, WordClass.Noun),
            new HaikuWord("forest", 2, WordClass.Noun),
            new HaikuWord("insight", 2, WordClass.Noun),
            new HaikuWord("mountain", 2, WordClass.Noun),
            new HaikuWord("nature", 2, WordClass.Noun),
            new HaikuWord("river", 2, WordClass.Noun),
            new HaikuWord("ocean", 2, WordClass.Noun),
            new HaikuWord("adventure", 3, WordClass.Noun),
            new HaikuWord("clarity", 3, WordClass.Noun),
            new HaikuWord("harmony", 3, WordClass.Noun),
            new HaikuWord("butterfly", 3, WordClass.Noun),
            new HaikuWord("mountainside", 3, WordClass.Noun),
            new HaikuWord("riverbed", 3, WordClass.Noun),
            new HaikuWord("waterfall", 3, WordClass.Noun),

            new HaikuWord("dreams", 1, WordClass.Verb),
            new HaikuWord("feels", 1, WordClass.Verb),
            new HaikuWord("loves", 1, WordClass.Verb),
            new HaikuWord("sails", 1, WordClass.Verb),
            new HaikuWord("sees", 1, WordClass.Verb),
            new HaikuWord("takes", 1, WordClass.Verb),
            new HaikuWord("walks", 1, WordClass.Verb),
            new HaikuWord("enters", 2, WordClass.Verb),
            new HaikuWord("explores", 2, WordClass.Verb),
            new HaikuWord("journeys", 2, WordClass.Verb),
            new HaikuWord("traverses", 2, WordClass.Verb),
            new HaikuWord("visits", 2, WordClass.Verb),
            new HaikuWord("wanders", 2, WordClass.Verb),
            new HaikuWord("discovers", 3, WordClass.Verb),
            new HaikuWord("meditates", 3, WordClass.Verb),

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
            new HaikuWord("angelic", 3, WordClass.Adjective),
            new HaikuWord("auspicious", 3, WordClass.Adjective),
            new HaikuWord("glorious", 3, WordClass.Adjective),
            new HaikuWord("picturesque", 3, WordClass.Adjective),

            new HaikuWord("quite", 1, WordClass.Adverb),
            new HaikuWord("soon", 1, WordClass.Adverb),
            new HaikuWord("well", 1, WordClass.Adverb),
            new HaikuWord("brightly", 2, WordClass.Adverb),
            new HaikuWord("today", 2, WordClass.Adverb),
            new HaikuWord("very", 2, WordClass.Adverb),
            new HaikuWord("lazily", 3, WordClass.Adverb),
            new HaikuWord("peacefully", 3, WordClass.Adverb),
            new HaikuWord("quietly", 3, WordClass.Adverb),
            new HaikuWord("tomorrow", 3, WordClass.Adverb),

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

            new HaikuWord("he", 1, WordClass.Pronoun),
            new HaikuWord("she", 1, WordClass.Pronoun),
            new HaikuWord("they", 1, WordClass.Pronoun),
        };

        public static List<List<(WordClass WordType, int Syllables)>> FiveSyllableSentences = new List<List<(WordClass WordType, int Syllables)>>
        {
            // --- Starting with articles ---
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1), (WordClass.Verb, 2) },      // "the strong wind blusters"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1), (WordClass.Verb, 1) },      // "a gentle wind blows"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 2), (WordClass.Verb, 1) },      // "the old river bends"
            new() { (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Adverb, 2) },         // "the sun sets slowly"
            new() { (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 2) },                                // "the river tumbles"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 2) },                           // "a silent shadow"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 3), (WordClass.Noun, 1) },                           // "a beautiful bird"
            new() { (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Adverb, 1), (WordClass.Verb, 2) },         // "the wind soon settles"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1), (WordClass.Preposition, 1), (WordClass.Noun, 1) }, // "a small bird in flight"
            new() { (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 2), (WordClass.Adverb, 1) },         // "the fire glimmers now"
 
            // --- Starting with nouns ---
            new() { (WordClass.Noun, 1), (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Noun, 1) },       // "wind whispers through leaves"
            new() { (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 1) }, // "birds sing in the trees"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Adjective, 2) },                              // "mountains stand alone"
            new() { (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 2), (WordClass.Noun, 1) },       // "clouds drift over hills"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },           // "silence fills the room"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Noun, 1) },       // "raindrops fall like tears"
            new() { (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Adverb, 3) },                                 // "birds fly gracefully"
            new() { (WordClass.Noun, 1), (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Noun, 1) },       // "light unfolds in time" (existing weighted pattern)
            new() { (WordClass.Noun, 2), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },    // "water in the well"
 
            // --- Starting with verbs (imperative) ---
            new() { (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },    // "listen to the rain"
            new() { (WordClass.Verb, 1), (WordClass.Preposition, 2), (WordClass.Article, 1), (WordClass.Noun, 1) },    // "wait beneath the moon"
            new() { (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 1) },           // "watch the river flow"
            new() { (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Adverb, 2), (WordClass.Adverb, 1) },   // "breathe in slowly now"
 
            // --- Starting with adjectives (poetic inversion) ---
            new() { (WordClass.Adjective, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },      // "silent falls the snow"
            new() { (WordClass.Adjective, 2), (WordClass.Noun, 2), (WordClass.Verb, 1) },                              // "gentle rivers flow"
            new() { (WordClass.Adjective, 2), (WordClass.Noun, 1), (WordClass.Verb, 2) },                              // "golden leaves descend"
 
            // --- Starting with adverbs ---
            new() { (WordClass.Adverb, 2), (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 1) },         // "slowly the sun fades"
            new() { (WordClass.Adverb, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },         // "softly falls the rain"
            new() { (WordClass.Adverb, 3), (WordClass.Noun, 1), (WordClass.Verb, 1) },                                 // "quietly night falls"
        };

        public static List<List<(WordClass WordType, int Syllables)>> SevenSyllableSentences = new List<List<(WordClass WordType, int Syllables)>>
        {
            // --- Starting with articles ---
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Adverb, 2) },                    // "a gentle breeze drifts slowly"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 2), (WordClass.Verb, 2), (WordClass.Adverb, 1) },                    // "the old mountain trembles now"
            new() { (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 2) }, // "the bird sits in the shadow"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Adverb, 1) },                    // "a silent river flows on"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 3), (WordClass.Noun, 1), (WordClass.Verb, 2) },                                           // "a beautiful bird ascends"
            new() { (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Noun, 2) },                    // "the river flows through canyons"
            new() { (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 2), (WordClass.Article, 1), (WordClass.Noun, 1) }, // "the sun sinks into the mist"
            new() { (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Noun, 2) }, // "the small bird sings in morning"
 
            // --- Starting with nouns ---
            new() { (WordClass.Noun, 1), (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 2) },                    // "wind whispers through the valley"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Preposition, 2), (WordClass.Article, 1), (WordClass.Noun, 1) },                    // "shadows fall beyond the hill"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Adjective, 1) },                      // "mountains guard the valley wide"
            new() { (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1) }, // "light falls on the golden field"
            new() { (WordClass.Noun, 2), (WordClass.Preposition, 2), (WordClass.Noun, 2), (WordClass.Verb, 1) },                                            // "thunder over mountains roars"
            new() { (WordClass.Noun, 2), (WordClass.Verb, 2), (WordClass.Adverb, 3) },                                                                      // "shadows gather endlessly"
 
            // --- Starting with pronouns ---
            new() { (WordClass.Pronoun, 1), (WordClass.Verb, 1), (WordClass.Adverb, 3), (WordClass.Preposition, 1), (WordClass.Noun, 1) },                  // "she walks gracefully through rain"
            new() { (WordClass.Pronoun, 1), (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 2) },                 // "they wander through the forest"
            new() { (WordClass.Pronoun, 1), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1), (WordClass.Adverb, 2) }, // "he sings a soft song sweetly"
            new() { (WordClass.Pronoun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 2), (WordClass.Article, 1), (WordClass.Noun, 2) },                 // "we sit beneath the willow"
            new() { (WordClass.Pronoun, 1), (WordClass.Adverb, 2), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Adjective, 1), (WordClass.Noun, 1) }, // "they slowly walk through deep snow"
 
            // --- Starting with verbs (imperative) ---
            new() { (WordClass.Verb, 2), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1) },               // "listen to the gentle rain"
            new() { (WordClass.Verb, 1), (WordClass.Adverb, 3), (WordClass.Preposition, 1), (WordClass.Article, 1), (WordClass.Noun, 1) },                  // "wait patiently for the dawn"
            new() { (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Adverb, 2) },                         // "watch the river flow gently"
            new() { (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1), (WordClass.Adverb, 2) },                // "breathe in golden light slowly"
            new() { (WordClass.Verb, 2), (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Preposition, 1), (WordClass.Noun, 2) },                    // "gather the wood for winter"
 
            // --- Starting with adjectives (poetic inversion) ---
            new() { (WordClass.Adjective, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Adverb, 1) },                    // "silent falls the twilight now"
            new() { (WordClass.Adjective, 2), (WordClass.Noun, 2), (WordClass.Verb, 2), (WordClass.Adverb, 1) },                                            // "golden mountains glisten now"
            new() { (WordClass.Adjective, 3), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Preposition, 1), (WordClass.Noun, 1) },                  // "beautiful birds soar through sky"
            new() { (WordClass.Adjective, 1), (WordClass.Adjective, 1), (WordClass.Noun, 2), (WordClass.Verb, 2), (WordClass.Adverb, 1) },                  // "cold gray mountains tremble now"
 
            // --- Starting with adverbs ---
            new() { (WordClass.Adverb, 3), (WordClass.Article, 1), (WordClass.Noun, 1), (WordClass.Verb, 1), (WordClass.Adjective, 1) },                    // "quietly the night grows cold"
            new() { (WordClass.Adverb, 2), (WordClass.Article, 1), (WordClass.Noun, 2), (WordClass.Verb, 1), (WordClass.Preposition, 1) },                  // "slowly the shadows creep in"
            new() { (WordClass.Adverb, 2), (WordClass.Verb, 1), (WordClass.Article, 1), (WordClass.Adjective, 2), (WordClass.Noun, 1) },                    // "softly falls the gentle rain"
        };

        /// <summary>
        /// Creates a string in a haiku (5-7-5) format.
        /// </summary>
        /// <returns></returns>
        public static string GenerateHaiku()
        {
            string haiku = string.Empty;
            Random random = new Random();

            // Create the string line by line
            for (int i = 0; i < 3; i++)
            {
                // Select a template
                List<(WordClass, int)> selectedTemplate = (i == 1 ? SevenSyllableSentences.ElementAt(Random.Shared.Next(SevenSyllableSentences.Count)) : FiveSyllableSentences.ElementAt(Random.Shared.Next(FiveSyllableSentences.Count)));

                // When the template has been chosen, go through each part of it and insert relevant words
                foreach (var templatePart in selectedTemplate)
                {
                    // Grab a word that has the same word class and syllable count as the template
                    var words = WordBank.Where(w => w.SyllableCount == templatePart.Item2 && w.WordClass == templatePart.Item1).ToList();
                    int wordIndex = Random.Shared.Next(words.Count);
                    HaikuWord selectedWord = words[wordIndex];

                    haiku += $"{selectedWord?.Word} ";
                }

                if (i < 2)
                    haiku += "| ";
            }

            return haiku;
        }
    }
}
