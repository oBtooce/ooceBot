using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Bits;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ooceBot.Commands
{
    /// <summary>
    /// This class is used as the parameter for the Command delegate method. It contains all information that would be needed to implement a command.
    /// </summary>
    public class CommandArgs
    {
        public TwitchClient Client { get; set; }

        public ChatMessage ChatMessage { get; set; }
        
        public SqliteConnection Connection { get; set; }

        public HttpClient NightbotSongRequestClient { get; set; }

        public string CommandQuantifier { get; set; }

        public Random Random { get; set; } = new Random();

        public CommandArgs(TwitchClient client, ChatMessage message, SqliteConnection connection, HttpClient nightbotSongRequestClient, string commandQuantifier)
        {
            Client = client;
            ChatMessage = message;
            Connection = connection;
            NightbotSongRequestClient = nightbotSongRequestClient;
            CommandQuantifier = commandQuantifier;
        }
    }
}
