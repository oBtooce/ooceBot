using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace ooceBot.SQL
{
    public static class DBQueryMethods
    {
        public static void PopulateCommandUsageTable(SqliteConnection connection, Dictionary<string, BotVariables.Command> videoCommands)
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.Parameters.Add("@commandName", SqliteType.Text);

            foreach (var cmd in videoCommands)
            {
                command.Parameters["@commandName"].Value = cmd.Key;
                command.CommandText = $"INSERT OR IGNORE INTO CommandUsage (commandID) VALUES (@commandName)";

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if the chatter exists in the DB and adds them if they do not
        /// </summary>
        /// <param name="connection">The SQLite database connection</param>
        /// <param name="username">The chatter's username</param>
        public static void VerifyExistenceInChattersTable(SqliteConnection connection, ChatMessage message)
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@chatter", message.DisplayName);
            command.Parameters.AddWithValue("@chatterid", message.UserId);

            command.CommandText = "SELECT * FROM Chatters WHERE userID = @chatterid OR username = @chatter LIMIT 1";

            // If no user was found for either the submitted ID and username, make a new record
            if (command.ExecuteScalar() == null)
            {
                command.CommandText = "INSERT INTO Chatters (userID, username) VALUES (@chatterid, @chatter)";
                command.ExecuteNonQuery();
            }
            else // Perform an update on the found record
            {
                command.CommandText = "UPDATE Chatters SET userID = @chatterid, username = @chatter WHERE userID = @chatterid OR username = @chatter";
                command.ExecuteNonQuery();
            }
        }
    }
}
