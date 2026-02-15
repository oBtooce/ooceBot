using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Charity.GetCharityCampaign;

namespace ooceBot.Miscellaneous
{
    public static class ArcadeMethods
    {
        /// <summary>
        /// Checks to see if the user is part of the arcade system
        /// </summary>
        /// <param name="connection">The SQLite connection object</param>
        /// <param name="userId">The user's unique Twitch ID</param>
        /// <returns></returns>
        public static bool CheckForPlayer(SqliteConnection connection, string userId)
        {
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", userId);

            command.CommandText = "SELECT * FROM ArcadeStats WHERE userID = @userId";

            return command.ExecuteScalar() != null;
        }

        public static bool DecideTokenOutcome(Random random)
        {
            const int UPPER_LIMIT = 100;
            const int OFFSET = 10;
            decimal midpoint = Math.Ceiling((decimal)(UPPER_LIMIT / 2)) - OFFSET;

            // Something really basic like a coinflip for now, but with a larger number set
            int value = random.Next(UPPER_LIMIT + 1); // 0 to 100, Next() does not include the specified upper limit, hence the +1 to include 100
            
            return value >= midpoint;
        }

        public static ArcadeStats GetPlayerCurrentStats(SqliteConnection connection, string userId)
        {
            var currentStats = new ArcadeStats();

            var chatterStatistics = connection.CreateCommand();
            chatterStatistics.Parameters.AddWithValue("@userId", userId);

            chatterStatistics.CommandText = "SELECT * FROM ArcadeStats WHERE userID = @userId";

            using (SqliteDataReader reader = chatterStatistics.ExecuteReader())
            {
                // We only want the first record since there will only ever be one returned record, so if is a fine replacement for while
                if (reader.Read())
                {
                    currentStats = new ArcadeStats()
                    {
                        TimesWagered = reader.GetInt32(reader.GetOrdinal("times_wagered")),
                        TotalTokens = reader.GetInt32(reader.GetOrdinal("total_tokens")),
                        LargestWager = reader.GetInt32(reader.GetOrdinal("largest_wager")),
                        HighScore = reader.GetInt32(reader.GetOrdinal("high_score")),
                        WinningStreak = reader.GetInt32(reader.GetOrdinal("winning_streak")),
                        LongestWinningStreak = reader.GetInt32(reader.GetOrdinal("longest_winning_streak")),
                        DidWinWager = true
                    };
                }
                else
                    throw new Exception("No suitable record found in database.");
            }

            return currentStats;
        }

        public static int GetTotalTokens(SqliteConnection Connection)
        {
            var tokenTotal = Connection.CreateCommand();

            tokenTotal.CommandText = $"SELECT total_tokens FROM ArcadeStats";

            return Convert.ToInt32(tokenTotal.ExecuteScalar());
        }

        public static void HandleBuyins(SqliteConnection connection, string userId, int defaultBuyin)
        {
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@defaultBuyin", defaultBuyin);

            command.CommandText = "SELECT total_tokens FROM ArcadeStats WHERE userID = @userId";

            // Check if the total token value is less than the default buy-in and add some tokens for playing
            if (Convert.ToInt32(command.ExecuteScalar()) < defaultBuyin)
            {
                command.CommandText = "UPDATE ArcadeStats SET total_tokens = @defaultBuyin WHERE userID = @userId";
                command.ExecuteNonQuery();
            }
        }

        public static void SetupPlayer(SqliteConnection connection, string userId)
        {
            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", userId);
            command.CommandText = "INSERT OR IGNORE INTO ArcadeStats (userID) VALUES (@userId)";

            command.ExecuteNonQuery();
        }

        public static void UpdateArcadeRecord(ref ArcadeStats currentStats, int wagerAmount, SqliteConnection connection, string userId)
        {
            var currentTotalTokens = currentStats.TotalTokens;
            var currentHighScore = currentStats.HighScore;
            var currentLongestStreak = currentStats.LongestWinningStreak;

            if (currentStats.DidWinWager)
            {
                // Update all required values (win)
                currentStats.TimesWagered++;
                currentStats.TotalTokens = currentTotalTokens + wagerAmount;
                currentStats.LargestWager = wagerAmount > currentStats.LargestWager ? wagerAmount : currentStats.LargestWager;
                currentStats.HighScore = currentStats.TotalTokens > currentHighScore ? currentStats.TotalTokens : currentHighScore;
                currentStats.WinningStreak++;
                currentStats.LongestWinningStreak = currentStats.WinningStreak > currentLongestStreak ? currentStats.WinningStreak : currentLongestStreak;
            }
            else
            {
                // Update all required values (loss)
                currentStats.TimesWagered++;
                currentStats.TotalTokens = currentTotalTokens - wagerAmount;
                currentStats.LargestWager = wagerAmount > currentStats.LargestWager ? wagerAmount : currentStats.LargestWager;
                currentStats.WinningStreak = 0;
            }

            var command = connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", userId);
            command.CommandText = $@"
    UPDATE ArcadeStats 
    SET 
        times_wagered = {currentStats.TimesWagered},
        total_tokens = {currentStats.TotalTokens},
        largest_wager = {currentStats.LargestWager},
        high_score = {currentStats.HighScore},
        winning_streak = {currentStats.WinningStreak},
        longest_winning_streak = {currentStats.LongestWinningStreak}
    WHERE userID = @userId";

            command.ExecuteNonQuery();
        }
    }
}
