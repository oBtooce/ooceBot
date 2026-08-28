using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class ArcadeStats
    {
        public string Id { get; set; }

        public int TimesWagered { get; set; }

        public int TotalTokens { get; set; }

        public int LargestWager { get; set; }

        public int HighScore { get; set; }

        public int WinningStreak { get; set; }

        public int LongestWinningStreak { get; set; }
    }
}
