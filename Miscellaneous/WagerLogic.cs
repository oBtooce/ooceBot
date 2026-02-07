using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Miscellaneous
{
    public static class WagerLogic
    {
        public static WagerRecord? DecideWagerOutcomeAndRecordResults(int amount, string displayName, Random random, SqliteConnection connection)
        {
            WagerRecord? currentValues = null;

            int upperLimit = 100;
            int midpoint = upperLimit % 2 == 0 ? upperLimit / 2 : upperLimit / 2 + 1;

            // Something really basic like a coinflip for now, but with a larger number set
            int value = random.Next(upperLimit + 1); // 0 to 100, Next() does not include the specified upper limit, hence the +1 to include 100
            bool didWinWager = value >= midpoint;

            if (didWinWager)
            {
                connection.Open();

                var chatterStatistics = connection.CreateCommand();
                chatterStatistics.CommandText = $"SELECT * FROM WagerStats WHERE username = {displayName}";
                
                using (SqliteDataReader reader = chatterStatistics.ExecuteReader())
                {
                    // We only want the first record since there will only ever be one returned record, so if is a fine replacement for while
                    if (reader.Read())
                    {
                        currentValues = new WagerRecord()
                        {
                            TimesWagered = reader.GetInt32(reader.GetOrdinal("times_wagered")),
                            TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points")),
                            LargestWager = reader.GetInt32(reader.GetOrdinal("largest_wager")),
                            HighScore = reader.GetInt32(reader.GetOrdinal("high_score")),
                            WinningStreak = reader.GetInt32(reader.GetOrdinal("winning_streak")),
                            LongestWinningStreak = reader.GetInt32(reader.GetOrdinal("longest_winning_streak")),
                            DidWinWager = true
                        };

                        var currentTotalPoints = currentValues.TotalPoints;
                        var currentHighScore = currentValues.HighScore;
                        var currentLongestStreak = currentValues.LongestWinningStreak;

                        // Update all required values
                        currentValues.TimesWagered++;
                        currentValues.TotalPoints = currentTotalPoints + amount;
                        currentValues.LargestWager = amount > currentValues.LargestWager ? amount : currentValues.LargestWager;
                        currentValues.IsLargestWager = amount > currentValues.LargestWager ? true : false;
                        currentValues.HighScore = currentValues.TotalPoints > currentHighScore ? currentValues.TotalPoints : currentHighScore;
                        currentValues.DidHighScoreIncrease = currentValues.HighScore > currentHighScore ? true : false;
                        currentValues.WinningStreak = currentValues.WinningStreak + 1;
                        currentValues.DidWinStreakIncrease = true;
                        currentValues.LongestWinningStreak = currentValues.WinningStreak > currentLongestStreak ? currentValues.WinningStreak : currentLongestStreak;
                        currentValues.DidLongestStreakIncrease = currentValues.LongestWinningStreak > currentLongestStreak ? true : false;
                    }
                    else
                        throw new Exception("No suitable record found in database.");
                }

                // Ensure that a wager record was populated accordingly and then populate the DB
                if (currentValues != null)
                {
                    chatterStatistics.CommandText = $@"
    UPDATE WagerStats 
    SET 
        times_wagered = {currentValues.TimesWagered},
        total_points = {currentValues.TotalPoints},
        largest_wager = {currentValues.LargestWager},
        high_score = {currentValues.HighScore},
        winning_streak = {currentValues.WinningStreak},
        longest_winning_streak = {currentValues.LongestWinningStreak}
    WHERE username = {displayName}";

                    chatterStatistics.ExecuteNonQuery();
                    connection.Close();

                    return currentValues;
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
                chatterStatistics.CommandText = $"SELECT * FROM WagerStats WHERE username = {displayName}";

                using (SqliteDataReader reader = chatterStatistics.ExecuteReader())
                {
                    // We only want the first record since there will only ever be one returned record, so if is a fine replacement for while
                    if (reader.Read())
                    {
                        currentValues = new WagerRecord()
                        {
                            TimesWagered = reader.GetInt32(reader.GetOrdinal("times_wagered")),
                            TotalPoints = reader.GetInt32(reader.GetOrdinal("total_points")),
                            LargestWager = reader.GetInt32(reader.GetOrdinal("largest_wager")),
                            HighScore = reader.GetInt32(reader.GetOrdinal("high_score")),
                            WinningStreak = reader.GetInt32(reader.GetOrdinal("winning_streak")),
                            LongestWinningStreak = reader.GetInt32(reader.GetOrdinal("longest_winning_streak")),
                            DidWinWager = false
                        };

                        var currentHighScore = currentValues.HighScore;

                        // Update all required values
                        currentValues.TimesWagered++;
                        currentValues.TotalPoints = currentValues.TotalPoints - amount;
                        currentValues.LargestWager = amount > currentValues.LargestWager ? amount : currentValues.LargestWager;
                        currentValues.IsLargestWager = false;
                        currentValues.HighScore = currentValues.TotalPoints > currentHighScore ? currentValues.TotalPoints : currentHighScore;
                        currentValues.DidHighScoreIncrease = false;
                        currentValues.WinningStreak = 0;
                        currentValues.DidWinStreakIncrease = false;
                        currentValues.LongestWinningStreak = currentValues.WinningStreak > currentValues.LongestWinningStreak ? currentValues.WinningStreak : currentValues.LongestWinningStreak;
                        currentValues.DidLongestStreakIncrease = false;
                    }
                    else
                        throw new Exception("No suitable record found in database.");
                }

                // Ensure that a wager record was populated accordingly and then populate the DB
                if (currentValues != null)
                {
                    chatterStatistics.CommandText = $@"
    UPDATE WagerStats 
    SET 
        times_wagered = {currentValues.TimesWagered},
        total_points = {currentValues.TotalPoints},
        largest_wager = {currentValues.LargestWager},
        high_score = {currentValues.HighScore},
        winning_streak = {currentValues.WinningStreak},
        longest_winning_streak = {currentValues.LongestWinningStreak}
    WHERE username = {displayName}";

                    chatterStatistics.ExecuteNonQuery();
                    connection.Close();

                    return currentValues;
                }
                else
                {
                    connection.Close();
                    return null;
                }
            }
        }
    }
}
