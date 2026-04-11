using ooceBot.Commands;
using ooceBot.Functionality;
using ooceBot.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Attendance
{
    public static class AttendanceMethods
    {
        public static void ProvideAttendanceInfo(CommandArgs args)
        {
            var command = args.Connection.CreateCommand();

            string message;

            switch (args.CommandQuantifier)
            {
                case "points":
                    command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

                    command.CommandText = $"SELECT PointsForRedemption FROM AttendanceRecords WHERE Id = @userId";

                    message = $"{args.ChatMessage.DisplayName}, you currently have {Convert.ToInt32(command.ExecuteScalar())} points to use on channel redemptions. Attend more streams to earn more points {BotVariables.obtoocBri}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    break;
                case "total":
                    command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

                    command.CommandText = $"SELECT TotalAttendance FROM AttendanceRecords WHERE Id = @userId";
                    
                    message = $"{args.ChatMessage.DisplayName}, you've attended {Convert.ToInt32(command.ExecuteScalar())} streams. You rock {BotVariables.obtoocBri}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    break;                
                default:
                    message = $"If you would like more attendance information, try the following available commands: !here total, !here points";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    break;
            }
        }

        public static void TakeAttendance(CommandArgs args)
        {
            string message;

            var obj = args.Context.AttendanceRecords.FirstOrDefault(rec => rec.Id == args.ChatMessage.UserId);

            var command = args.Connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);
            command.Parameters.AddWithValue("@pointValue", BotVariables.ATTENDANCE_POINT_VALUE);

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            command.Parameters.AddWithValue("@streamStartTime", BotVariables.StreamStartTime);

            var chatterUserID = args.ChatMessage.UserId;
            var chatterDisplayName = args.ChatMessage.DisplayName;

            DBQueryMethods.VerifyExistenceInChattersTable(args.Connection, args.ChatMessage);

            // Check to see if user has declared their presence today and in the stream (accounts for streams that go past the midnight mark)
            command.CommandText = "SELECT IsPresent, LastPresentDate FROM AttendanceRecords WHERE Id = @userId";

            bool alreadyPresent = false;

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    long isPresent = reader.GetInt64(0);
                    string lastPresentDate = reader.IsDBNull(1) ? null : reader.GetString(1);

                    alreadyPresent = (isPresent == 1 || (lastPresentDate == BotVariables.StreamStartTime || lastPresentDate == today));
                }
            }

            // If attendance was already taken, then prevent it from happening
            if (alreadyPresent)
            {
                message = $"Your attendance has already been taken. Check in next time {BotVariables.obtoocBri}";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                return;
            }

            // Create a new attendance record or update an existing one
            command.CommandText = $@"
                    INSERT INTO AttendanceRecords (Id, AttendanceCount, TotalAttendance, IsPresent, LastPresentDate) VALUES (@userId, 1, 1, 1, @streamStartTime)
                    ON CONFLICT(Id)
                    DO UPDATE SET AttendanceCount = AttendanceCount + 1, TotalAttendance = TotalAttendance + 1, IsPresent = 1, LastPresentDate = @streamStartTime
                ";

            command.ExecuteNonQuery();

            // Get the relevant attendance total from the DB
            command.CommandText = $"SELECT AttendanceCount FROM AttendanceRecords WHERE Id = @userId";
            int attendanceCount = Convert.ToInt32(command.ExecuteScalar());
            int daysInClass = attendanceCount % 10;

            // When reaching 10 days, reward with points
            if (daysInClass == 0)
            {
                message = $"{BotVariables.obtoocW} {BotVariables.obtoocW} Congratulations! {BotVariables.obtoocW} {BotVariables.obtoocW}    {chatterDisplayName}, to reward you for your regular attendance, you get {BotVariables.ATTENDANCE_POINT_VALUE} \"points\" to spend on channel point redemptions {BotVariables.obtoocBri}";

                command.CommandText = $"UPDATE AttendanceRecords SET AttendanceCount = 0, PointsForRedemption = PointsForRedemption + @pointValue WHERE Id = @userId";
                command.ExecuteNonQuery();
            }
            else
                message = $"{chatterDisplayName}, your attendance has been recorded. You have {daysInClass} {(daysInClass == 1 ? "day" : "days")} on record. Let's see what happens when you reach 10 days {BotVariables.obtoocBri}";

            // Let 'em know
            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }
    }
}
