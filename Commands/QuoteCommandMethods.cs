using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Commands
{
    public static class QuoteCommandMethods
    {
        public static int AddQuote(string quote)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string quoteFilePath = $"{currentDirectory}/quoteFile.txt";

            // Create the file if it doesn't already exist
            if (!File.Exists(quoteFilePath))
                File.Create(quoteFilePath);

            // Add newline to delimit the quote
            File.AppendText($"{quote}\n");

            // Return the index for users to try out
            return File.ReadLines(quoteFilePath).Count();
        }
    }
}
