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
                    userID TEXT PRIMARY KEY,
                    username TEXT
                )
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
                CREATE TABLE IF NOT EXISTS ChatterAttendance (
                    userID TEXT PRIMARY KEY,
                    attendance_count INTEGER DEFAULT 0,
                    total_attendance INTEGER NOT NULL DEFAULT 0,
                    is_present INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (userID) REFERENCES Chatters(userID)
                )
            ";

            command.ExecuteNonQuery();

            // Attendance is reset for the day
            command.CommandText = "UPDATE ChatterAttendance SET is_present = 0";
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
                CREATE TABLE IF NOT EXISTS ArcadeStats (
                    userID TEXT PRIMARY KEY,
                    times_wagered INTEGER NOT NULL DEFAULT 0,
                    total_tokens INTEGER NOT NULL DEFAULT 0,
                    largest_wager INTEGER NOT NULL DEFAULT 0,
                    high_score INTEGER NOT NULL DEFAULT 0,
                    winning_streak INTEGER NOT NULL DEFAULT 0,
                    longest_winning_streak INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (userID) REFERENCES Chatters(userID)
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
                    commandID TEXT PRIMARY KEY,
                    usage_count INTEGER NOT NULL DEFAULT 0                  
                )
            ";

            command.ExecuteNonQuery();

            // Reset command usage counts back to default
            command.CommandText = "UPDATE CommandUsage SET usage_count = 0";
            command.ExecuteNonQuery();
        }
    }
}
