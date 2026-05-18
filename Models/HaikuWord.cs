using ooceBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class HaikuWord
    {
        public HaikuWord(string word, int syllableCount, WordClass wordClass)
        {
            Word = word;
            SyllableCount = syllableCount;
            WordClass = wordClass;
        }

        public string Word { get; set; }

        public int SyllableCount { get; set; }

        public WordClass WordClass { get; set; }
    }
}
