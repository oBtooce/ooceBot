using OBSWebsocketDotNet;
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
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Communication.Interfaces;

namespace ooceBot.Commands
{
    public static class CommandMethods
    {
        public static void AddQuote(CommandArgs args)
        {
            if (args.CommandQuantifier != string.Empty)
            {
                QuoteCommandMethods.AddQuote(args.CommandQuantifier);
                args.Client.SendMessage(args.ChatMessage.Channel, $"Quote added. Thank you for creating history in oBtooce's stream!");
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

        public static void Boner(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"don't get married");
        }

        public static void BuyIn(CommandArgs args)
        {
            //Add new user or ignore if ID is already present
            ArcadeMethods.SetupPlayer(args.Connection, args.ChatMessage.UserId);

            // Check balances for player and provide tokens if balance is empty
            ArcadeMethods.HandleBuyins(args.Connection, args.ChatMessage.UserId, BotVariables.DEFAULT_BUYIN);

            args.Client.SendMessage(args.ChatMessage.Channel, $"{args.ChatMessage.DisplayName}, you now have {BotVariables.DEFAULT_BUYIN} tokens to play with. Have fun obtoocBri");
        }

        public static void Croissant(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"https://en.wikipedia.org/wiki/En_passant");
        }

        public static void Discord(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's Discord: https://discord.gg/5RTxTFurGF");
        }

        public static void Emotes(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "Follower emotes: obtoocBri obtoocF obtoocW obtoocNice obtoocOmg");
        }
        public static void Exclaim(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocBri obtoocBri obtoocBri obtoocBri obtoocBri");
        }

        public static void F(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocF obtoocF obtoocF obtoocF obtoocF");
        }

        public static void FineCheddar(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"don't get married");
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

        public static async void Guts(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                // Get the current volume from the API
                int originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

                // Keep track of the volume for the reset after the video is done
                int updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, originalVolume);

                PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

                // Reset the volume to its previous level
                await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, originalVolume);
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }

        public static void Help(CommandArgs args)
        {
            // Standard help command
            if (string.IsNullOrEmpty(args.CommandQuantifier))
            {
                // Build up a string of command to share with the chat
                string commandListMessage = "Try out any of these commands: ";
                List<string> Keys = Program.CommandsList.Keys.OrderBy(k => k).ToList();

                foreach (var key in Keys)
                    commandListMessage += key == Keys.Last() ? key : $"{key} | ";

                args.Client.SendMessage(args.ChatMessage.Channel, commandListMessage);
            }
            else if (Program.CommandsList.Keys.Contains(args.CommandQuantifier))
            {
                // Do some stuff
                // Note for later: think about storing each command as the text without the ! and add that later
            }
            else
            {
                args.Client.SendMessage(args.ChatMessage.Channel, "That command does not exist, but your enthusiasm is noted obtoocBri");
            }
        }

        public static void Here(CommandArgs args)
        {
            var command = args.Connection.CreateCommand();
            command.Parameters.AddWithValue("@userId", args.ChatMessage.UserId);

            var chatterUserID = args.ChatMessage.UserId;
            var chatterDisplayName = args.ChatMessage.DisplayName;

            DBQueryMethods.VerifyExistenceInChattersTable(args.Connection, args.ChatMessage);

            // Initial check to see if user has declared their presence today
            command.CommandText = $"SELECT is_present FROM ChatterAttendance WHERE userID = @userId";
            var attendanceTakenValue = command.ExecuteScalar();

            // If the user exists but attendance was already taken, then prevent it from happening again
            if (attendanceTakenValue != null && (long)attendanceTakenValue == 1)
            {
                args.Client.SendMessage(args.ChatMessage.Channel, "Your attendance has already been taken. Check in next time obtoocBri");
                return;
            }

            // Create a new attendance record or update an existing one
            command.CommandText = $@"
                        INSERT INTO ChatterAttendance (userID, attendance_count, is_present) VALUES (@userId, 1, 1)
                        ON CONFLICT(userID)
                        DO UPDATE SET attendance_count = attendance_count + 1, is_present = 1
                    ";

            command.ExecuteNonQuery();

            // Get the relevant attendance total from the DB
            command.CommandText = $"SELECT attendance_count FROM ChatterAttendance WHERE userID = @userId";
            int attendanceCount = Convert.ToInt32(command.ExecuteScalar());

            string message;
            int daysInClass = attendanceCount % 10;

            if (daysInClass == 0)
                message = $"obtoocW obtoocW Congratulations! obtoocW obtoocW    {chatterDisplayName}, to reward you for your regular attendance, you get to redeem a channel point reward for free (up to a value of 2000 points) obtoocBri";
            else
                message = $"{chatterDisplayName}, your attendance has been recorded. You have {daysInClass} {(daysInClass == 1 ? "day" : "days")} on record. Let's see what happens when you reach 10 days obtoocBri";

            // Let 'em know
            args.Client.SendMessage(args.ChatMessage.Channel, message);
        }

        public static void Jacob(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"Blackjack");
        }

        public static void Lurk(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"{args.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon obtoocBri");
        }

        public static void Nice(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocNice obtoocNice obtoocNice obtoocNice obtoocNice");
        }

        public static void Play(CommandArgs args)
        {
            // Make sure that the user exists before doing anything
            if (!ArcadeMethods.CheckForPlayer(args.Connection, args.ChatMessage.UserId))
            {
                args.Client.SendMessage(args.ChatMessage.Channel, $"Looks like this is your first time at the arcade. Type !buyin to get your first set of tokens obtoocBri");
                return;
            }

            string arcadeTokens = args.CommandQuantifier;

            if (arcadeTokens.EndsWith("%") && decimal.TryParse(arcadeTokens.Split("%").First(), out decimal percentageValue) == true && percentageValue > 0 && percentageValue <= 100) // Percentage amount
            {
                int totalTokens = ArcadeMethods.GetTotalTokens(args.Connection);

                // Get the proper token value from the percentage
                decimal percentAsNumber = percentageValue / 100;
                int tokenValueFromPercentage = (int)(percentAsNumber * totalTokens);

                // Calculate whether or not the wager won or lost
                ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(args.Connection, args.ChatMessage.UserId);
                arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(args.Random);

                ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenValueFromPercentage, args.Connection, args.ChatMessage.UserId);

                if (arcadeRecord.DidWinWager)
                    args.Client.SendMessage(args.ChatMessage.Channel, $"obtoocW Nice win, {args.ChatMessage.DisplayName}! obtoocW Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.");
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, $"Oof...no luck this time, {args.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.");
            }
            else if (int.TryParse(arcadeTokens, out int tokenAmount) && tokenAmount > 0) // Flat token amount
            {
                // Verify if the token amount is within the user's total token stash and proceed if so, else jump out with a warning
                int totalTokens = ArcadeMethods.GetTotalTokens(args.Connection);

                if (totalTokens > tokenAmount)
                {
                    // Calculate whether or not the wager won or lost
                    ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(args.Connection, args.ChatMessage.UserId);
                    arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(args.Random);

                    ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenAmount, args.Connection, args.ChatMessage.UserId);

                    if (arcadeRecord.DidWinWager)
                        args.Client.SendMessage(args.ChatMessage.Channel, $"obtoocW Nice win, {args.ChatMessage.DisplayName}! obtoocW Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.");
                    else
                        args.Client.SendMessage(args.ChatMessage.Channel, $"Oof...no luck this time, {args.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.");
                }
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, "You don't have enough tokens to make that bet. Try again later obtoocBri");
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, "This seems...off. Try again with either a number (50) or a valid percentage (50%).");
        }

        public static void Quote(CommandArgs args)
        {
            // Check if a line number has been provided and validate it, otherwise return a random quote from the text file
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
            {
                var isNumeric = int.TryParse(args.CommandQuantifier, out int result);

                if (isNumeric == false)
                {
                    args.Client.SendMessage(args.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number. Otherwise, just type !randomquote or !rq for a random quote.");
                    return;
                }

                // If we got a valid number, do the work
                var lines = File.ReadAllLines(QuoteCommandMethods.FilePath).Length;

                if (result < 0 || result >= lines)
                    args.Client.SendMessage(args.ChatMessage.Channel, $"Whoops! that number is out of range. Try a number from 0 to {lines - 1}");
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, QuoteCommandMethods.SelectQuote(result));
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
        }

        public static void RandomQuote(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, QuoteCommandMethods.SelectQuote());
        }

        public static void RNGMove(CommandArgs args)
        {
            if (args.ChatMessage.IsBroadcaster || args.ChatMessage.IsModerator)
            {
                string move = $"{BotVariables.PIECE_NOTATION[args.Random.Next(BotVariables.PIECE_NOTATION.Length)]}{BotVariables.FILE_NOTATION[args.Random.Next(BotVariables.FILE_NOTATION.Length)]}{BotVariables.RANK_NOTATION[args.Random.Next(BotVariables.RANK_NOTATION.Length)]}";

                args.Client.SendMessage(args.ChatMessage.Channel, $"The move for next game is {move} obtoocBri");
            }
        }

        public static async void Salute(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                // Get the current volume from the API
                int originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

                // Keep track of the volume for the reset after the video is done
                int updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, originalVolume);

                PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Beautiful Trumpet.mp3", 2000, 2000);

                // Reset the volume to its previous level
                await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, originalVolume);
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }

        public static void Schedule(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's schedule is a complete lie. Just tune in whenever!");
        }

        public static void Spotify(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's Spotify page: https://open.spotify.com/user/obtoose");
        }

        public static void Stats(CommandArgs args)
        {
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
                ChessCommandMethods.GetChesscomStats(args.Client, args.ChatMessage, args.CommandQuantifier);
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !stats (username)");
        }

        public static void Steam(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's Steam page: https://steamcommunity.com/id/obtooce/");
        }

        public static void Tarf(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, $"you gotta be bad, you gotta be bold, you gotta be wiser, you gotta be hard, you gotta be tough, you gotta be stronger, you gotta be cool, you gotta be calm, you gotta stay together, all i know love will save the day - corrected");
        }

        public static async void Title(CommandArgs args)
        {
            if (!string.IsNullOrEmpty(args.CommandQuantifier))
            {
                TwitchAPI api = new TwitchAPI();

                // Update the API settings with client ID and OAuth token
                api.Settings.ClientId = ConfigurationManager.AppSettings["TwitchClientID"];
                api.Settings.AccessToken = BotVariables.TwitchOAuthToken.Split(":").Last(); // To work with TwitchAPI, the access token can not have the "oauth:" prefix, so we chop it off

                // Get the broadcaster ID for the channel modification request
                var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> { BotVariables.ChannelToJoin });
                string broadcasterId = users.Users[0].Id;

                await api.Helix.Channels.ModifyChannelInformationAsync(broadcasterId, new ModifyChannelInformationRequest { Title = args.CommandQuantifier });

                args.Client.SendMessage(args.ChatMessage.Channel, $"Title has been updated to \"{args.CommandQuantifier}\"");
            }
        }

        public static void Twitter(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's Twitter: https://twitter.com/oBtuuse");
        }

        public static void Vid(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "Latest YouTube video: https://youtu.be/STmFRwBFvqc");
        }

        public static void W(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "obtoocW obtoocW obtoocW obtoocW obtoocW");
        }

        public static async void WTF(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                // Get the current volume from the API
                int volume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);

                // Keep track of the volume for the reset after the video is done
                volume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, volume, 50);

                // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                var currentScene = websocket.GetCurrentProgramScene();
                var sceneItems = websocket.GetSceneItemList(currentScene);

                // Need to figure out a way to make the source name not a string because this is a bad setup
                var wtfScene = sceneItems.First(item => item.SourceName == "WTF");

                await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, wtfScene);

                // Reset the volume after the video is done
                await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, volume, 50);
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }

        public static void YouTube(CommandArgs args)
        {
            args.Client.SendMessage(args.ChatMessage.Channel, "oBtooce's YouTube channel: https://www.youtube.com/channel/UCjS2ciB4D3iftZS3Hj1CCWg");
        }
    }
}
