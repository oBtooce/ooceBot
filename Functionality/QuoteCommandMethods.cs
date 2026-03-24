using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Functionality
{
    public static class QuoteCommandMethods
    {
        public static Random random { get; set; } = new Random();

        public static string FilePath { get; set; } = $"{Directory.GetCurrentDirectory()}/quoteFile.txt";

        /// <summary>
        /// Adds a new quote to the quote file
        /// </summary>
        /// <param name="quote"></param>
        public static void AddQuote(string quote)
        {
            // Create the file if it doesn't already exist
            if (!File.Exists(FilePath))
                File.Create(FilePath);

            // Add newline to delimit the quote
            File.AppendAllText(FilePath, $"#{File.ReadAllLines(FilePath).Length + 1}: {quote + Environment.NewLine}");
        }

        public static string SelectQuote(int index = -1)
        {
            string[] quoteLines = File.ReadAllLines(FilePath);

            // When we have a value, make sure it is valid
            if (index != -1)
            {
                if (index > quoteLines.Length)
                    return string.Empty;
                else
                    return quoteLines[index];
            }
            else
            {
                var randomIndex = random.Next(0, quoteLines.Length);
                return quoteLines[randomIndex];
            }
        }
    }
}
