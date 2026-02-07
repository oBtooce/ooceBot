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

            // Initialize the tables needed for stream
            InitializeChatterTable(connection);
            InitializeAttendanceTable(connection);
            InitializeWageringTable(connection);

            // Close the connection to the DB
            connection.Close();
        }

        /// <summary>
        /// Creates a table that is used for taking attendance.
        /// </summary>
        public static void InitializeChatterTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Chatters (
                    username TEXT PRIMARY KEY
                )
            ";

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates a table that is used for taking attendance.
        /// </summary>
        public static void InitializeAttendanceTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ChatterAttendance (
                    username TEXT,
                    attendance_count INTEGER DEFAULT 0,
                    is_present INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (username) REFERENCES Chatters(username)
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
        public static void InitializeWageringTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();

            // Table creation
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS WagerStats (
                    username TEXT,
                    times_wagered INTEGER NOT NULL DEFAULT 0,
                    total_points INTEGER NOT NULL DEFAULT 0,
                    largest_wager INTEGER NOT NULL DEFAULT 0,
                    high_score INTEGER NOT NULL DEFAULT 0,
                    winning_streak INTEGER NOT NULL DEFAULT 0,
                    longest_winning_streak INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (username) REFERENCES Chatters(username)
                )
            ";

            command.ExecuteNonQuery();
        }
    }
}
