using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Commands
{
    public static class QuoteCommandMethods
    {
        public static Random random { get; set; } = new Random();

        public static void AddQuote(string quote)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/quoteFile.txt";

            // Create the file if it doesn't already exist
            if (!File.Exists(filePath))
                File.Create(filePath);

            // Add newline to delimit the quote
            File.AppendAllText(filePath, quote + Environment.NewLine);
        }

        public static string SelectQuote(int index = -1)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/quoteFile.txt";
            string[] quoteLines = File.ReadAllLines(filePath);

            if (index != -1)
                return quoteLines[index];
            else
            {
                var randomIndex = random.Next(0, quoteLines.Length - 1);
                return quoteLines[randomIndex];
            }
        }
    }
}
