using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OBSWebsocketDotNet;
using ooceBot.Authorization;
using ooceBot.Commands;
using ooceBot.Functionality;
using ooceBot.Models;
using ooceBot.Sounds;
using ooceBot.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.ChannelPoints;

namespace ooceBot.AttendanceLogic
{
    public static class AttendanceMethods
    {
        public static void TakeAttendance(CommandArgs args)
        {
            string message;

            var attendanceRecord = args.Context.AttendanceRecords.FirstOrDefault(attendance => attendance.Id == args.ChatMessage.UserId);

            if (attendanceRecord is not null)
            {
                // If there is a value in the DB and said value is the same as the current stream start time, prevent attendance
                if (attendanceRecord.DateOfAttendance == BotVariables.StreamStartTime)
                {
                    message = $"Your attendance has already been taken. Check in next time {BotVariables.obtoocBri}";
                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    return;
                }

                // Update all relevant values
                attendanceRecord.AttendanceCount++;
                attendanceRecord.TotalAttendance++;
                attendanceRecord.DateOfAttendance = BotVariables.StreamStartTime;

                // If the tenth day has been reached, reward accordingly!
                if (attendanceRecord.AttendanceCount % 10 == 0)
                {
                    attendanceRecord.AttendanceCount %= 10;
                    attendanceRecord.PointsForRedemption += BotVariables.ATTENDANCE_POINT_VALUE;

                    message = $"{BotVariables.obtoocW} {BotVariables.obtoocW} Congratulations! {BotVariables.obtoocW} {BotVariables.obtoocW}    {args.ChatMessage.DisplayName}, to reward you for your regular attendance, you get {BotVariables.ATTENDANCE_POINT_VALUE} \"points\" to spend on channel point redemptions {BotVariables.obtoocBri} Your current total is {attendanceRecord.PointsForRedemption}";
                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
                else
                {
                    if (args.CommandText == "!present")
                        message = $"Insubordinate and churlish! {args.ChatMessage.DisplayName}, your attendance is noted ({attendanceRecord.AttendanceCount}/10), but follow the rules next time! You've attended a total of {attendanceRecord.TotalAttendance} {(attendanceRecord.TotalAttendance == 1 ? "stream" : "streams")}.";
                    else
                        message = $"{args.ChatMessage.DisplayName}, your attendance has been recorded ({attendanceRecord.AttendanceCount}/10). You've attended a total of {attendanceRecord.TotalAttendance} {(attendanceRecord.TotalAttendance == 1 ? "stream" : "streams")} {BotVariables.obtoocBri}";

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
                    PointsForRedemption = 0,
                    DateOfAttendance = BotVariables.StreamStartTime
                };

                message = $"{args.ChatMessage.DisplayName}, your attendance journey has begun {BotVariables.obtoocBri}";
                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);

                args.Context.AttendanceRecords.Add(newAttendance);
            }

            // Save changes after all is said and done
            args.Context.SaveChanges();
        }

        public static async Task RedeemReward(CustomReward reward, Attendance attendanceRecord, CommandArgs args)
        {
            string rewardName = reward.Title;

            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            int originalVolume = await VolumeControl.GetNightbotCurrentVolume(Program.NightbotSongRequestClient);
            int volumeChange;

            switch (rewardName)
            {
                case "Lobster":
                    volumeChange = (int)(originalVolume * 0.9);
                    await EventSubWebsocketManager.PlayVideoInOBS(websocket, originalVolume, volumeChange, "LOBSTER");
                    break;
                case "Who Do You Think You Are!?":
                    volumeChange = (int)(originalVolume * 0.9);
                    await EventSubWebsocketManager.PlayVideoInOBS(websocket, originalVolume, volumeChange, "WHO");
                    break;
                case "WTF":
                    volumeChange = (int)(originalVolume * 0.9);
                    await EventSubWebsocketManager.PlayVideoInOBS(websocket, originalVolume, volumeChange, "WTF");
                    break;
                case "Something To Make You Smile :)":
                    volumeChange = (int)(originalVolume * 0.9);
                    await EventSubWebsocketManager.PlayVideoInOBS(websocket, originalVolume, volumeChange, "Maggie");
                    break;
                case "The Cure For Sadness...":
                    volumeChange = (int)(originalVolume * 0.9);
                    await EventSubWebsocketManager.PlayVideoInOBS(websocket, originalVolume, volumeChange, "Homer");
                    break;
            }

            attendanceRecord.PointsForRedemption -= reward.Cost;
            args.Context.SaveChanges();
        }
    }
}
