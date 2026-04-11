using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                command.CommandText = $"INSERT OR IGNORE INTO CommandUsage (Id) VALUES (@commandName)";

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

            command.CommandText = "SELECT * FROM Chatters WHERE Id = @chatterid OR DisplayName = @chatter LIMIT 1";

            // If no user was found for either the submitted ID and username, make a new record
            if (command.ExecuteScalar() == null)
            {
                command.CommandText = "INSERT INTO Chatters (Id, DisplayName) VALUES (@chatterid, @chatter)";
                command.ExecuteNonQuery();
            }
            else // Perform an update on the found record
            {
                command.CommandText = "UPDATE Chatters SET Id = @chatterid, DisplayName = @chatter WHERE Id = @chatterid OR DisplayName = @chatter";
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateChatterDataPlusMaybeTheme(SqliteConnection connection, ChatMessage message)
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", message.UserId);
            command.Parameters.AddWithValue("@username", message.Username);

            command.CommandText = $@"
                INSERT INTO Chatters (Id, DisplayName, HasTheme, HasChattedThisStream) VALUES (@userId, @username, 0, 1)
                ON CONFLICT(userID)
                DO UPDATE SET HasChattedThisStream = 1
            ";

            //using (SqliteDataReader reader = command.ExecuteReader())
            //{
            //    // Since Sqlite does not have a bool primitive, we check for 1 from the return
            //    var hasCustomIntro = reader.GetInt32(reader.GetOrdinal("HasTheme"));

            //    if (hasCustomIntro == 1)
            //    {
            //        // do work here
            //    }
            //}
        }
    }
}
