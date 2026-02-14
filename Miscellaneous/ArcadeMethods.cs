using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Miscellaneous
{
    public static class ArcadeMethods
    {
        public static ArcadeStats? DecideTokenOutcome(int amount, string displayName, Random random, SqliteConnection connection)
        {
            ArcadeStats? currentStats = null;

            const int UPPER_LIMIT = 100;
            decimal midpoint = Math.Ceiling((decimal)(UPPER_LIMIT / 2));

            // Something really basic like a coinflip for now, but with a larger number set
            int value = random.Next(UPPER_LIMIT + 1); // 0 to 100, Next() does not include the specified upper limit, hence the +1 to include 100
            bool didWinWager = value >= midpoint;

            if (didWinWager)
            {
                connection.Open();

                var chatterStatistics = connection.CreateCommand();
                chatterStatistics.CommandText = $"SELECT * FROM ArcadeStats WHERE username = {displayName}";
                
                using (SqliteDataReader reader = chatterStatistics.ExecuteReader())
                {
                    // We only want the first record since there will only ever be one returned record, so if is a fine replacement for while
                    if (reader.Read())
                    {
                        currentStats = new ArcadeStats()
                        {
                            TimesWagered = reader.GetInt32(reader.GetOrdinal("times_wagered")),
                            TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points")),
                            LargestWager = reader.GetInt32(reader.GetOrdinal("largest_wager")),
                            HighScore = reader.GetInt32(reader.GetOrdinal("high_score")),
                            WinningStreak = reader.GetInt32(reader.GetOrdinal("winning_streak")),
                            LongestWinningStreak = reader.GetInt32(reader.GetOrdinal("longest_winning_streak")),
                            DidWinWager = true
                        };

                        var currentTotalPoints = currentStats.TotalPoints;
                        var currentHighScore = currentStats.HighScore;
                        var currentLongestStreak = currentStats.LongestWinningStreak;

                        // Update all required values
                        currentStats.TimesWagered++;
                        currentStats.TotalPoints = currentTotalPoints + amount;
                        currentStats.LargestWager = amount > currentStats.LargestWager ? amount : currentStats.LargestWager;
                        currentStats.IsLargestWager = amount > currentStats.LargestWager ? true : false;
                        currentStats.HighScore = currentStats.TotalPoints > currentHighScore ? currentStats.TotalPoints : currentHighScore;
                        currentStats.DidHighScoreIncrease = currentStats.HighScore > currentHighScore ? true : false;
                        currentStats.WinningStreak = currentStats.WinningStreak + 1;
                        currentStats.DidWinStreakIncrease = true;
                        currentStats.LongestWinningStreak = currentStats.WinningStreak > currentLongestStreak ? currentStats.WinningStreak : currentLongestStreak;
                        currentStats.DidLongestStreakIncrease = currentStats.LongestWinningStreak > currentLongestStreak ? true : false;
                    }
                    else
                        throw new Exception("No suitable record found in database.");
                }

                // Ensure that a wager record was populated accordingly and then populate the DB
                if (currentStats != null)
                {
                    chatterStatistics.CommandText = $@"
    UPDATE ArcadeStats 
    SET 
        times_wagered = {currentStats.TimesWagered},
        total_points = {currentStats.TotalPoints},
        largest_wager = {currentStats.LargestWager},
        high_score = {currentStats.HighScore},
        winning_streak = {currentStats.WinningStreak},
        longest_winning_streak = {currentStats.LongestWinningStreak}
    WHERE username = {displayName}";

                    chatterStatistics.ExecuteNonQuery();
                    connection.Close();

                    return currentStats;
                }
                else
                {
                    connection.Close();
                    return null;
                }
            }
            else // We lost the wager AAAAAAAAAAAAAAAAAAAAAAAAAAA
            {
                connection.Open();

                var chatterStatistics = connection.CreateCommand();
                chatterStatistics.CommandText = $"SELECT * FROM ArcadeStats WHERE username = {displayName}";

                using (SqliteDataReader reader = chatterStatistics.ExecuteReader())
                {
                    // We only want the first record since there will only ever be one returned record, so if is a fine replacement for while
                    if (reader.Read())
                    {
                        currentStats = new ArcadeStats()
                        {
                            TimesWagered = reader.GetInt32(reader.GetOrdinal("times_wagered")),
                            TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points")),
                            LargestWager = reader.GetInt32(reader.GetOrdinal("largest_wager")),
                            HighScore = reader.GetInt32(reader.GetOrdinal("high_score")),
                            WinningStreak = reader.GetInt32(reader.GetOrdinal("winning_streak")),
                            LongestWinningStreak = reader.GetInt32(reader.GetOrdinal("longest_winning_streak")),
                            DidWinWager = false
                        };

                        var currentHighScore = currentStats.HighScore;

                        // Update all required values
                        currentStats.TimesWagered++;
                        currentStats.TotalPoints = currentStats.TotalPoints - amount;
                        currentStats.LargestWager = amount > currentStats.LargestWager ? amount : currentStats.LargestWager;
                        currentStats.IsLargestWager = false;
                        currentStats.HighScore = currentStats.TotalPoints > currentHighScore ? currentStats.TotalPoints : currentHighScore;
                        currentStats.DidHighScoreIncrease = false;
                        currentStats.WinningStreak = 0;
                        currentStats.DidWinStreakIncrease = false;
                        currentStats.LongestWinningStreak = currentStats.WinningStreak > currentStats.LongestWinningStreak ? currentStats.WinningStreak : currentStats.LongestWinningStreak;
                        currentStats.DidLongestStreakIncrease = false;
                    }
                    else
                        throw new Exception("No suitable record found in database.");
                }

                // Ensure that a wager record was populated accordingly and then populate the DB
                if (currentStats != null)
                {
                    chatterStatistics.CommandText = $@"
    UPDATE currentStats 
    SET 
        times_wagered = {currentStats.TimesWagered},
        total_points = {currentStats.TotalPoints},
        largest_wager = {currentStats.LargestWager},
        high_score = {currentStats.HighScore},
        winning_streak = {currentStats.WinningStreak},
        longest_winning_streak = {currentStats.LongestWinningStreak}
    WHERE username = {displayName}";

                    chatterStatistics.ExecuteNonQuery();
                    connection.Close();

                    return currentStats;
                }
                else
                {
                    connection.Close();
                    return null;
                }
            }
        }

        public static int GetTotalTokens(SqliteConnection Connection)
        {
            var tokenTotal = Connection.CreateCommand();

            tokenTotal.CommandText = $"SELECT total_points FROM ArcadeStats";

            return Convert.ToInt32(tokenTotal.ExecuteScalar());
        }
    }
}
