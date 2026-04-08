using Microsoft.Data.Sqlite;
using OBSWebsocketDotNet;
using ooceBot.Attendance;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using ooceBot.Functionality;
using ooceBot.Miscellaneous;
using ooceBot.Sounds;
using ooceBot.SQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix.Models.Soundtrack;
using TwitchLib.Communication.Interfaces;
using static System.Formats.Asn1.AsnWriter;

namespace ooceBot.Commands
{
    public static class CommandMethods
    {
        public static void AddQuote(CommandArgs args)
        {
            if (args.CommandQuantifier != string.Empty)
            {
                QuoteCommandMethods.AddQuote(args.CommandQuantifier);
                args.Client.SendMessage(args.ChatMessage.Channel, $"Quote added. Thank you for creating history in the stream {BotVariables.obtoocBri}");
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"When using the !addquote command, don't forget to include the quote! The command looks like this: !addquote \"insert quote here\"");
        }

        public static void Audit(CommandArgs args)
        {
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
                ChessCommandMethods.AuditChatter(args.Client, args.ChatMessage, args.CommandQuantifier);
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !audit (username)");
        }        

        public static void Based(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, @"    ____                      __
   / __ )____ _________  ____/ /
  / __  / __ `/ ___/ _ \/ __  / 
 / /_/ / /_/ (__  )  __/ /_/ /  
/_____/\__,_/____/\___/\__,_/   
                                ");
        }

        public static void Bird(CommandArgs args)
        {
            var command = args.Connection.CreateCommand();

            command.CommandText = $"UPDATE Miscellaneous SET bird_counter = bird_counter + 1 RETURNING bird_counter";

            int birdCount = Convert.ToInt32(command.ExecuteScalar());

            string message = $"The Bird Opening has been used {birdCount} {(birdCount == 1 ? "time" : "times")} {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Boner(CommandArgs args)
        {
            string message = $"don't get married";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void BuyIn(CommandArgs args)
        {
            //Add new user or ignore if ID is already present
            DBQueryMethods.VerifyExistenceInChattersTable(args.Connection, args.ChatMessage);
            ArcadeMethods.SetupPlayer(args.Connection, args.ChatMessage.UserId);

            // Check balances for player and provide tokens if balance is empty
            ArcadeMethods.HandleBuyins(args.Connection, args.ChatMessage.UserId, BotVariables.DEFAULT_BUYIN);

            string message = $"{args.ChatMessage.DisplayName}, you have been given {BotVariables.DEFAULT_BUYIN} tokens. Have fun {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Croissant(CommandArgs args)
        {
            string message = $"https://en.wikipedia.org/wiki/En_passant";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Discord(CommandArgs args)
        {
            string message = $"oBtooce's Discord: {BotVariables.DiscordLink}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Emotes(CommandArgs args)
        {
            string message = $"Follower emotes: {BotVariables.obtoocBri} {BotVariables.obtoocF} {BotVariables.obtoocW} {BotVariables.obtoocNice} {BotVariables.obtoocOmg}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Exclaim(CommandArgs args)
        {
            string message = $"{BotVariables.obtoocBri} {BotVariables.obtoocBri} {BotVariables.obtoocBri} {BotVariables.obtoocBri} {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void FineCheddar(CommandArgs args)
        {
            string message = $"Some of the finest cheese can be found here: https://en.wikipedia.org/wiki/Fianchetto";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static async void Groove(CommandArgs args)
        {
            //if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            //{
            //    // Get the current volume from the API
            //    int originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

            //    // Keep track of the volume for the reset after the video is done
            //    int updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, originalVolume);

            //    // Pull a random song from a tunes folder
            //    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

            //    // Reset the volume to its previous level
            //    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, originalVolume);
            //}
            //else
            //    args.Client.SendMessage(args.ChatMessage.Channel, "Sorry baby, it looks like you ain't got the dancin' fever. Show some more passion to get your one-way ticket to Melody Town, ya dig?");
        }

        public static void Help(CommandArgs args)
        {
            // Format the incoming string to handle ! and casing
            string formattedCommandText = (args.CommandQuantifier ?? "").TrimStart('!').ToLower();

            // Standard help command
            if (string.IsNullOrEmpty(formattedCommandText))
            {
                // Build up a string of commands to share with the chat
                string commandListMessage = "";
                List<string> Keys = BotVariables.CommandsList.Keys.OrderBy(k => k).ToList();

                foreach (var key in Keys)
                    commandListMessage += key == Keys.Last() ? key : $"{key} • ";

                commandListMessage += " [Video commands for subs/VIPs: ";

                List<string> VideoKeys = BotVariables.VideoCommands.Keys.OrderBy(vk => vk).ToList();

                foreach (var key in VideoKeys)
                    commandListMessage += key == VideoKeys.Last() ? $"{key}]" : $"{key} • ";

                string message = commandListMessage;

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
            else if (BotVariables.CommandDictionary.ContainsKey(formattedCommandText))
            {
                string message = BotVariables.CommandDictionary[formattedCommandText];

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
            else
            {
                string message = $"That command does not exist, but your enthusiasm is noted {BotVariables.obtoocBri}";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Here(CommandArgs args)
        {
            if (args.CommandQuantifier == string.Empty)
                AttendanceMethods.TakeAttendance(args);
            else
                AttendanceMethods.ProvideAttendanceInfo(args);
        }

        public static void Jacob(CommandArgs args)
        {
            string message = $"Blackjack";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Lurk(CommandArgs args)
        {
            string message = $"{args.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Play(CommandArgs args)
        {
            string message;
            // Make sure that the user exists before doing anything
            if (!ArcadeMethods.CheckForPlayer(args.Connection, args.ChatMessage.UserId))
            {
                message = $"Looks like this is your first time at the arcade. Type !buyin to get your first set of tokens {BotVariables.obtoocBri}";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                return;
            }

            string arcadeTokens = args.CommandQuantifier;

            if (arcadeTokens.EndsWith("%") && decimal.TryParse(arcadeTokens.Split("%").First(), out decimal percentageValue) == true && percentageValue > 0 && percentageValue <= 100) // Percentage amount
            {
                int totalTokens = ArcadeMethods.GetTotalTokens(args.Connection, args.ChatMessage.UserId);

                // Get the proper token value from the percentage
                decimal percentAsNumber = percentageValue / 100;
                int tokenValueFromPercentage = (int)(percentAsNumber * totalTokens);

                // Calculate whether or not the wager won or lost
                ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(args.Connection, args.ChatMessage.UserId);
                arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(args.Random);

                ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenValueFromPercentage, args.Connection, args.ChatMessage.UserId);

                if (arcadeRecord.DidWinWager)
                {
                    message = $"{BotVariables.obtoocW} Nice win, {args.ChatMessage.DisplayName}! {BotVariables.obtoocW} Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }                    
                else
                {
                    message = $"Oof...no luck this time, {args.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else if (int.TryParse(arcadeTokens, out int tokenAmount) && tokenAmount > 0) // Flat token amount
            {
                // Verify if the token amount is within the user's total token stash and proceed if so, else jump out with a warning
                int totalTokens = ArcadeMethods.GetTotalTokens(args.Connection, args.ChatMessage.UserId);

                if (totalTokens >= tokenAmount)
                {
                    // Calculate whether or not the wager won or lost
                    ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(args.Connection, args.ChatMessage.UserId);
                    arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(args.Random);

                    ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenAmount, args.Connection, args.ChatMessage.UserId);

                    if (arcadeRecord.DidWinWager)
                    {
                        message = $"{BotVariables.obtoocW} Nice win, {args.ChatMessage.DisplayName}! {BotVariables.obtoocW} Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.";

                        args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    }
                    else
                    {
                        message = $"Oof...no luck this time, {args.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.";

                        args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    }
                }
                else
                {
                    message = $"You don't have enough tokens to make that bet. To see your current token total, type !tokens {BotVariables.obtoocBri}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                message = "This seems...off. Try again with either a number (50) or a valid percentage (50%).";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Quote(CommandArgs args)
        {
            string message;

            // Check if a line number has been provided and validate it, otherwise return a random quote from the text file
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
            {
                var isNumeric = int.TryParse(args.CommandQuantifier, out int result);

                if (isNumeric == false)
                {
                    message = "If you are choosing a quote, make sure you enter a number. Otherwise, just type !randomquote or !rq for a random quote.";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    return;
                }

                // If we got a valid number, do the work
                var lines = File.ReadAllLines(QuoteCommandMethods.FilePath).Length;

                if (result < 0 || result >= lines)
                {
                    message = $"Whoops! that number is out of range. Try a number from 0 to {lines - 1}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
                else
                {
                    message = QuoteCommandMethods.SelectQuote(result);

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                message = "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void RandomQuote(CommandArgs args)
        {
            string message = QuoteCommandMethods.SelectQuote();

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Redeem(CommandArgs args)
        {
            string message;

            // Three things to check: there is text; the text translates to a number; and the number is found within the dictionary
            if (!string.IsNullOrEmpty(args.CommandQuantifier) && int.TryParse(args.CommandQuantifier, out int key) && BotVariables.CustomRewards.TryGetValue(key, out CustomReward reward))
            {
                var command = args.Connection.CreateCommand();
                command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

                command.CommandText = $"SELECT points_for_redemption FROM ChatterAttendance WHERE userID = @userId";
                var attendancePoints = command.ExecuteScalar();

                if (attendancePoints != null && (long)attendancePoints > reward.Cost)
                {
                    var updatedPoints = (long)attendancePoints - reward.Cost;

                    command.CommandText = $"UPDATE ChatterAttendance SET points_for_redemption = {updatedPoints} WHERE userID = @userId";
                    command.ExecuteNonQuery();

                    message = $"{args.ChatMessage.DisplayName}, you redeemed \"{reward.Title}\" for {reward.Cost} points. Your remaining total is {updatedPoints}. Thanks for hanging out in chat {BotVariables.obtoocBri}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
                else
                {
                    message = $"{args.ChatMessage.DisplayName}, you do not have enough points to afford that reward. Pick something else {BotVariables.obtoocBri}";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                message = $"To redeem a reward, please enter a number from 1 to {BotVariables.CustomRewards.Count}. You can find the list of rewards here: {ConfigurationManager.AppSettings["GitHubGistUrl"]}";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Rewards(CommandArgs args)
        {
            string message = $"Check out all channel point rewards here: {ConfigurationManager.AppSettings["GitHubGistUrl"]}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Rule(CommandArgs args)
        {
            string message = $"Donating 250 bits allows you to make a \"rule\" that oBtooce has to follow for 5 minutes. Any rules deemed to be \"unfit\" will be ignored, so make it count {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message); ;
        }

        public static async void Salute(CommandArgs args)
        {
            string message;

            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!BotVariables.IsAudioOrVideoPlaying)
                {
                    // Get the current volume from the API
                    double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, originalVolume);

                    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Beautiful Trumpet.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else
                {
                    message = $"Gotta wait 'til the other stuff is done playing.";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                message = $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Schedule(CommandArgs args)
        {
            string message = "oBtooce's schedule is a complete lie. Just tune in whenever!";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static async void Sorrow(CommandArgs args)
        {
            string message;

            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!BotVariables.IsAudioOrVideoPlaying)
                {
                    // Get the current volume from the API
                    double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, originalVolume);

                    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else
                {
                    message = $"Gotta wait 'til the other stuff is done playing.";

                    args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                }
            }
            else
            {
                message = $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Spotify(CommandArgs args)
        {
            string message = $"oBtooce's Spotify page: {BotVariables.SpotifyPage}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Stats(CommandArgs args)
        {
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
                ChessCommandMethods.GetChesscomStats(args.Client, args.ChatMessage, args.CommandQuantifier);
            else
            {
                string message = $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !stats (username)";

                args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
            }
        }

        public static void Steam(CommandArgs args)
        {
            string message = $"oBtooce's Steam page: {BotVariables.SteamPage}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Store(CommandArgs args)
        {
            string message = $"Nothing at the store yet. Stay tuned {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Tarf(CommandArgs args)
        {
            string message = $"you gotta be bad, you gotta be bold, you gotta be wiser, you gotta be hard, you gotta be tough, you gotta be stronger, you gotta be cool, you gotta be calm, you gotta stay together, all i know love will save the day - corrected";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Tokens(CommandArgs args)
        {
            var command = args.Connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

            command.CommandText = $"SELECT total_tokens FROM ArcadeStats WHERE userID = @userId";
            var totalTokens = command.ExecuteScalar();

            string message = $"{args.ChatMessage.DisplayName}, you have {totalTokens} tokens {BotVariables.obtoocBri}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void TopPlayers(CommandArgs args)
        {
            StringBuilder chatMessage = new StringBuilder();
            int position = 1;

            var command = args.Connection.CreateCommand();

            command.CommandText = $"SELECT Chatters.username, ArcadeStats.total_tokens FROM Chatters INNER JOIN ArcadeStats ON Chatters.userID = ArcadeStats.userID WHERE ArcadeStats.total_tokens > 0";

            // Todo: Figure out some way to possibly create a Twitch extension that I can use to show the top player leaderboard since Twitch does not allow for line breaks
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    chatMessage.Append($"{position}: {reader[0]} - {reader[1]} points");
                    position++;
                }
            }

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(chatMessage.ToString()) : chatMessage.ToString());
        }

        public static void Twitter(CommandArgs args)
        {
            string message = $"oBtooce's Twitter: {BotVariables.TwitterPage}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }

        public static void Vid(CommandArgs args)
        {
            string message = $"Latest YouTube video: {BotVariables.LatestYTVideo}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }        

        public static void YouTube(CommandArgs args)
        {
            string message = $"oBtooce's YouTube channel: {BotVariables.YouTubeChannel}";

            args.Client.SendMessage(args.ChatMessage.Channel, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
        }
    }
}
