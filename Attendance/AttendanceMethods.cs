using ooceBot.Commands;
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

            switch (args.CommandQuantifier)
            {
                case "points":
                    command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

                    command.CommandText = $"SELECT points_for_redemption FROM ChatterAttendance WHERE userID = @userId";

                    args.Client.SendMessage(BotVariables.ChannelToJoin, $"{args.ChatMessage.DisplayName}, you currently have {Convert.ToInt32(command.ExecuteScalar())} points to use on channel redemptions. Attend more streams to earn more points {BotVariables.obtoocBri}");
                    break;
                case "total":
                    command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

                    command.CommandText = $"SELECT total_attendance FROM ChatterAttendance WHERE userID = @userId";
                    
                    args.Client.SendMessage(BotVariables.ChannelToJoin, $"{args.ChatMessage.DisplayName}, you've attended {Convert.ToInt32(command.ExecuteScalar())} streams. You rock {BotVariables.obtoocBri}");
                    break;                
                default:
                    args.Client.SendMessage(BotVariables.ChannelToJoin, $"If you would like more attendance information, try the following available commands: !here total, !here points");
                    break;
            }
        }

        public static void TakeAttendance(CommandArgs args)
        {
            var command = args.Connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);
            command.Parameters.AddWithValue("@pointValue", BotVariables.ATTENDANCE_POINT_VALUE);

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            command.Parameters.AddWithValue("@streamStartTime", BotVariables.StreamStartTime);

            var chatterUserID = args.ChatMessage.UserId;
            var chatterDisplayName = args.ChatMessage.DisplayName;

            DBQueryMethods.VerifyExistenceInChattersTable(args.Connection, args.ChatMessage);

            // Check to see if user has declared their presence today and in the stream (accounts for streams that go past the midnight mark)
            command.CommandText = "SELECT is_present, last_present_date FROM ChatterAttendance WHERE userID = @userId";

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
                args.Client.SendMessage(args.ChatMessage.Channel, $"Your attendance has already been taken. Check in next time {BotVariables.obtoocBri}");
                return;
            }

            // Create a new attendance record or update an existing one
            command.CommandText = $@"
                    INSERT INTO ChatterAttendance (userID, attendance_count, total_attendance, is_present, last_present_date) VALUES (@userId, 1, 1, 1, @streamStartTime)
                    ON CONFLICT(userID)
                    DO UPDATE SET attendance_count = attendance_count + 1, total_attendance = total_attendance + 1, is_present = 1, last_present_date = @streamStartTime
                ";

            command.ExecuteNonQuery();

            // Get the relevant attendance total from the DB
            command.CommandText = $"SELECT attendance_count FROM ChatterAttendance WHERE userID = @userId";
            int attendanceCount = Convert.ToInt32(command.ExecuteScalar());

            string message;
            int daysInClass = attendanceCount % 10;

            // When reaching 10 days, reward with points
            if (daysInClass == 0)
            {
                message = $"{BotVariables.obtoocW} {BotVariables.obtoocW} Congratulations! {BotVariables.obtoocW} {BotVariables.obtoocW}    {chatterDisplayName}, to reward you for your regular attendance, you get {BotVariables.ATTENDANCE_POINT_VALUE} \"points\" to spend on channel point redemptions {BotVariables.obtoocBri}";

                command.CommandText = $"UPDATE ChatterAttendance SET attendance_count = 0, points_for_redemption = points_for_redemption + @pointValue WHERE userID = @userId";
                command.ExecuteNonQuery();
            }
            else
                message = $"{chatterDisplayName}, your attendance has been recorded. You have {daysInClass} {(daysInClass == 1 ? "day" : "days")} on record. Let's see what happens when you reach 10 days {BotVariables.obtoocBri}";

            // Let 'em know
            args.Client.SendMessage(args.ChatMessage.Channel, message);
        }
    }
}
