using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.SQL
{
    public static class TableSQLMethods
    {
        public static void InitializeAllTables(SqliteConnection connection)
        {
            // Open the connection to the DB
            connection.Open();

            // Initialize the tables needed for stream (chatters table first since other tables use ID as their PK)
            InitializeChatterTable(connection);

            InitializeAttendanceTable(connection);
            InitializeWageringTable(connection);
            InitializeCommandUsageTable(connection);
            InitializeDapStatsTable(connection);

            InitializeMiscellaneousTable(connection);

            // Close the connection to the DB
            connection.Close();
        }

        /// <summary>
        /// Creates a table that is used for taking attendance.
        /// </summary>
        /// <param name="connection">The SQLite connection required for DB interaction</param>
        public static void InitializeChatterTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Chatters (
                    Id TEXT PRIMARY KEY,
                    DisplayName TEXT,
                    HasTheme INTEGER NOT NULL DEFAULT 0,
                    HasChattedThisStream INTEGER NOT NULL DEFAULT 0
                );

                CREATE INDEX IF NOT EXISTS idx_displayname_lower ON Chatters (LOWER(DisplayName));
            ";

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a table that is used for taking attendance.
        /// </summary>
        /// <param name="connection">The SQLite connection required for DB interaction</param>
        public static void InitializeAttendanceTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AttendanceRecords (
                    Id TEXT PRIMARY KEY,
                    AttendanceCount INTEGER NOT NULL DEFAULT 0,
                    TotalAttendance INTEGER NOT NULL DEFAULT 0,
                    IsPresent INTEGER NOT NULL DEFAULT 0,
                    LastPresentDate TEXT DEFAULT NULL,
                    PointsForRedemption INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (Id) REFERENCES Chatters(Id)
                )
            ";

            command.ExecuteNonQuery();

            // Attendance is reset for the day
            command.CommandText = "UPDATE AttendanceRecords SET IsPresent = 0";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a table that is used for wagering.
        /// </summary>
        /// <param name="connection">The SQLite connection required for DB interaction</param>
        public static void InitializeWageringTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ArcadeRecords (
                    Id TEXT PRIMARY KEY,
                    TimesWagered INTEGER NOT NULL DEFAULT 0,
                    TotalTokens INTEGER NOT NULL DEFAULT 0,
                    LargestWager INTEGER NOT NULL DEFAULT 0,
                    HighScore INTEGER NOT NULL DEFAULT 0,
                    WinningStreak INTEGER NOT NULL DEFAULT 0,
                    LongestWinningStreak INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (Id) REFERENCES Chatters(Id)
                )
            ";

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a table that is used to handle the number of times a video has been used through commands.
        /// </summary>
        /// <param name="connection">The SQLite connection required for DB interaction</param>
        public static void InitializeCommandUsageTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS CommandUsage (
                    Id TEXT PRIMARY KEY,
                    UsageCount INTEGER NOT NULL DEFAULT 0                  
                )
            ";

            command.ExecuteNonQuery();

            // Reset command usage counts back to default
            command.CommandText = "UPDATE CommandUsage SET UsageCount = 0";
            command.ExecuteNonQuery();
        }

        public static void InitializeDapStatsTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS DapRecords (
                    Id TEXT PRIMARY KEY,
                    DapsGiven INTEGER NOT NULL DEFAULT 0,
                    DapsReceived INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (Id) REFERENCES Chatters (Id)
                )
            ";

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a table that is used to handle miscellaneous values such as general counters.
        /// </summary>
        /// <param name="connection">The SQLite connection required for DB interaction</param>
        public static void InitializeMiscellaneousTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS GeneralStreamData (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    BirdCounter INTEGER NOT NULL DEFAULT 0
                );

                INSERT OR IGNORE INTO GeneralStreamData (Id, BirdCounter) VALUES (1, 0);
            ";

            command.ExecuteNonQuery();
        }
    }
}
