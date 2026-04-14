using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ooceBot.Commands;
using ooceBot.Functionality;
using ooceBot.Models;
using ooceBot.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.AttendanceLogic
{
    public static class AttendanceMethods
    {
        public static void ProvideAttendanceInfo(CommandArgs args)
        {
            string message;
            var attendanceRecord = args.Context.AttendanceRecords.FirstOrDefault(rec => rec.Id == args.ChatMessage.UserId);

            if (attendanceRecord is not null)
            {
                switch (args.CommandQuantifier)
                {
                    case "points":
                        message = $"{args.ChatMessage.DisplayName}, you currently have {attendanceRecord.PointsForRedemption} points to use on channel redemptions. Attend more streams to earn more points {BotVariables.obtoocBri}";
                        args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                        break;
                    case "total":
                        message = $"{args.ChatMessage.DisplayName}, you've attended {attendanceRecord.TotalAttendance} streams. You rock {BotVariables.obtoocBri}";
                        args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                        break;
                    default:
                        message = $"If you would like more attendance information, try one of the the following available commands in chat: !here points, !here total";
                        args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                        break;
                }
            }
            else
            {
                message = $"{args.ChatMessage.DisplayName}, have you seriously never attended this stream before!? Type !here to get started {BotVariables.obtoocBri}";
                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }            
        }

        public static void TakeAttendance(CommandArgs args)
        {
            string message;

            var attendanceRecord = args.Context.AttendanceRecords.FirstOrDefault(attendance => attendance.Id == args.ChatMessage.UserId);

            if (attendanceRecord is not null)
            {
                // Jump out if attendance was already taken (check for command used, then check for date matching)
                if (attendanceRecord.IsPresent == true || (attendanceRecord.LastPresentDate is not null && attendanceRecord.LastPresentDate.Value.ToString("yyyy-MM-dd") == BotVariables.StreamStartTime))
                {
                    message = $"Your attendance has already been taken. Check in next time {BotVariables.obtoocBri}";
                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    return;
                }

                // Update all relevant values
                attendanceRecord.AttendanceCount++;
                attendanceRecord.TotalAttendance++;
                attendanceRecord.IsPresent = true;
                attendanceRecord.LastPresentDate = DateOnly.FromDateTime(DateTimeOffset.Now.DateTime);

                // If the tenth day has been reached, reward accordingly!
                if (attendanceRecord.AttendanceCount % 10 == 0)
                {
                    attendanceRecord.AttendanceCount %= 10;

                    message = $"{BotVariables.obtoocW} {BotVariables.obtoocW} Congratulations! {BotVariables.obtoocW} {BotVariables.obtoocW}    {args.ChatMessage.DisplayName}, to reward you for your regular attendance, you get {BotVariables.ATTENDANCE_POINT_VALUE} \"points\" to spend on channel point redemptions {BotVariables.obtoocBri}";
                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
                else
                {
                    message = $"{args.ChatMessage.DisplayName}, your attendance has been recorded. You have {attendanceRecord.AttendanceCount} {(attendanceRecord.AttendanceCount == 1 ? "day" : "days")} on record. Let's see what happens when you reach 10 days {BotVariables.obtoocBri}";
                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                Attendance newAttendance = new Attendance
                {
                    Id = args.ChatMessage.UserId,
                    AttendanceCount = 1,
                    TotalAttendance = 1,
                    IsPresent = true,
                    PointsForRedemption = 0,
                    LastPresentDate = DateOnly.FromDateTime(DateTimeOffset.Now.DateTime)
                };

                message = $"{args.ChatMessage.DisplayName}, your attendance journey has begun! Let's see what happens when you reach 10 days {BotVariables.obtoocBri}";
                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);

                args.Context.AttendanceRecords.Add(newAttendance);
            }

            // Save changes after all is said and done
            args.Context.SaveChanges();
        }
    }
}
