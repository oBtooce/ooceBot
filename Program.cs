using Microsoft.VisualBasic;
using Microsoft.Data.Sqlite;
using System;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using ooceBot;
using System.Net.Http;
using System.Text.Json;
using ooceBot.Commands;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Helix;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using OBSWebsocketDotNet;
using System.Diagnostics;
using System.Net.WebSockets;
using OBSWebsocketDotNet.Types;
using ooceBot.Authorization;
using TwitchLib.PubSub.Models.Responses;
using ooceBot.Miscellaneous;
using ooceBot.SQL;
using ooceBot.Sounds;
using ooceBot.AudioVideo;
using System.Configuration;
using System.Runtime.CompilerServices;
using TwitchLib.Api.Helix.Models.Charity.GetCharityCampaign;

class Program
{
    private static TwitchClient Client { get; set; }

    private static HttpClient NightbotSongRequestClient { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    private static Random Random { get; set; } = new Random();

    public static async Task Main(string[] args)
    {
        // Create all tables that can be used through Twitch chat
        TableSQLMethods.InitializeAllTables(Connection);

        // Set access tokens for Nightbot and Twitch
        //await NighbotOAuthManager.SetNightbotOAuthToken();
        await TwitchOAuthManager.SetTwitchOAuthToken();

        // Set up Nightbot song requests client
        NightbotSongRequestClient = new HttpClient()
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["NightbotAPIRequestUri"]!)
        };

        NightbotSongRequestClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["NightbotOAuthToken"]);

        // Set up client
        ConnectionCredentials credentials = new ConnectionCredentials(BotVariables.BotUsername, BotVariables.TwitchOAuthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        var customClient = new WebSocketClient(clientOptions);
        Client = new TwitchClient(customClient);
        Client.Initialize(credentials);

        Client.OnConnected += Client_OnConnected;
        Client.OnMessageReceived += Client_OnMessageReceived;

        Client.OnError += (s, e) => Console.WriteLine("Error: " + e.Exception.Message);

        Client.OnLog += (s, e) =>
        {
            Console.WriteLine($"{e.DateTime:HH:mm:ss} {e.BotUsername} - {e.Data}");
        };

        Client.Connect();
        Console.ReadLine();
    }

    private static void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Client.JoinChannel(BotVariables.ChannelToJoin);
    }

    private static async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string[] messageParts = e.ChatMessage.Message.Split(new char[] { ' ' }, 2);

        // Open new connection
        Connection.Open();

        var command = Connection.CreateCommand();
        command.Parameters.AddWithValue("@userId", e.ChatMessage.UserId);

        switch (messageParts.First().ToLower())
        {
            case "!addquote":
                if (messageParts.Last() != string.Empty)
                {
                    QuoteCommandMethods.AddQuote(messageParts.Last());
                    Client.SendMessage(e.ChatMessage.Channel, $"Quote added. Thank you for creating history in oBtooce's stream!");
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"When using the !addquote command, don't forget to include the quote! The command looks like this: !addquote \"insert quote here\"");
                break;
            case "!audit":
                if (messageParts[1] != null)
                    ChessCommandMethods.AuditChatter(Client, e, messageParts.Last());
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !audit (username)");
                break;            
            case "!boner":
                Client.SendMessage(e.ChatMessage.Channel, $"don't get married");
                break;
            case "!buyin":
                // Add new user or ignore if ID is already present
                ArcadeMethods.SetupPlayer(Connection, e.ChatMessage.UserId);

                // Check balances for player and provide tokens if balance is empty
                ArcadeMethods.HandleBuyins(Connection, e.ChatMessage.UserId, BotVariables.DEFAULT_BUYIN);

                Client.SendMessage(e.ChatMessage.Channel, $"{e.ChatMessage.DisplayName}, you now have {BotVariables.DEFAULT_BUYIN} tokens to play with. Have fun obtoocBri");
                break;
            case "!commands":
                break;
            case "!croissant":
                Client.SendMessage(e.ChatMessage.Channel, $"https://en.wikipedia.org/wiki/En_passant");
                break;
            case "!dc":
            case "!discord":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's Discord: https://discord.gg/5RTxTFurGF");
                break;
            case "!emotes":
                Client.SendMessage(e.ChatMessage.Channel, "Follower emotes: obtoocBri obtoocF obtoocW obtoocNice obtoocOmg");
                break;
            case "!finecheddar":
                Client.SendMessage(e.ChatMessage.Channel, $"https://en.wikipedia.org/wiki/Fianchetto");
                break;
            case "!groove":
                if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    // Get the current volume from the API
                    int originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    int updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, originalVolume);

                    // Pull a random song from a tunes folder
                    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundFolder"]}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else                
                    Client.SendMessage(e.ChatMessage.Channel, "Sorry baby, it looks like you ain't got the dancin' fever. Show some more passion to get your one-way ticket to Melody Town, ya dig?");                
                break;
            case "!guts":
                if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    // Get the current volume from the API
                    int originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    int updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, originalVolume);

                    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundFolder"]}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");

                break;
            case "!here":
                var chatterUserID = e.ChatMessage.UserId;
                var chatterDisplayName = e.ChatMessage.DisplayName;

                DBQueryMethods.VerifyExistenceInChattersTable(Connection, e.ChatMessage);

                // Initial check to see if user has declared their presence today
                command.CommandText = $"SELECT is_present FROM ChatterAttendance WHERE userID = @userId";
                var attendanceTakenValue = command.ExecuteScalar();

                // If the user exists but attendance was already taken, then prevent it from happening again
                if (attendanceTakenValue != null && (long)attendanceTakenValue == 1)
                {
                    Client.SendMessage(e.ChatMessage.Channel, "Your attendance has already been taken. Check in next time obtoocBri");
                    return;
                }

                // Create a new attendance record or update an existing one
                command.CommandText = $@"
                    INSERT INTO ChatterAttendance (userID, attendance_count, is_present) VALUES (@chatterid, 1, 1)
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
                Client.SendMessage(e.ChatMessage.Channel, message);
                break;
            case "!jacob":
                Client.SendMessage(e.ChatMessage.Channel, $"Blackjack");
                break;
            case "!lurk":
                Client.SendMessage(e.ChatMessage.Channel, $"{e.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon obtoocBri");
                break;
            case "!play":
                // Make sure that the user exists before doing anything
                if (!ArcadeMethods.CheckForPlayer(Connection, e.ChatMessage.UserId))
                {
                    Client.SendMessage(e.ChatMessage.Channel, $"Looks like this is your first time at the arcade. Type !buyin to get your first set of tokens obtoocBri");
                    break;
                }

                string arcadeTokens = messageParts.Last();

                if (arcadeTokens.EndsWith("%") && int.TryParse(arcadeTokens.Split("%").First(), out int percentageValue) == true && percentageValue > 0 && percentageValue <= 100) // Percentage amount
                {
                    int totalTokens = ArcadeMethods.GetTotalTokens(Connection);

                    // Get the proper token value from the percentage
                    int tokenValueFromPercentage = (percentageValue / 100) * totalTokens;

                    // Calculate whether or not the wager won or lost
                    ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(Connection, e.ChatMessage.UserId);
                    arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(Random);

                    ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenValueFromPercentage, Connection, e.ChatMessage.UserId);

                    if (arcadeRecord.DidWinWager)
                        Client.SendMessage(e.ChatMessage.Channel, $"obtoocW Nice win, {e.ChatMessage.DisplayName}! obtoocW Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.");
                    else
                        Client.SendMessage(e.ChatMessage.Channel, $"Oof...no luck this time, {e.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.");
                }
                else if (int.TryParse(arcadeTokens, out int tokenAmount) == true) // Flat token amount
                {
                    // Verify if the token amount is within the user's total token stash and proceed if so, else jump out with a warning
                    int totalTokens = ArcadeMethods.GetTotalTokens(Connection);
                                        
                    if (totalTokens > tokenAmount)
                    {
                        // Calculate whether or not the wager won or lost
                        ArcadeStats arcadeRecord = ArcadeMethods.GetPlayerCurrentStats(Connection, e.ChatMessage.UserId);
                        arcadeRecord.DidWinWager = ArcadeMethods.DecideTokenOutcome(Random);

                        ArcadeMethods.UpdateArcadeRecord(ref arcadeRecord, tokenAmount, Connection, e.ChatMessage.UserId);

                        if (arcadeRecord.DidWinWager)
                            Client.SendMessage(e.ChatMessage.Channel, $"obtoocW Nice win, {e.ChatMessage.DisplayName}! obtoocW Looks like you've got {arcadeRecord.TotalTokens} tokens to spend.");
                        else
                            Client.SendMessage(e.ChatMessage.Channel, $"Oof...no luck this time, {e.ChatMessage.DisplayName}. Your new token total is {arcadeRecord.TotalTokens}.");
                    }
                    else
                        Client.SendMessage(e.ChatMessage.Channel, "You don't have enough tokens to make that bet. Try again later obtoocBri");
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, "This wager seems...off. Try again with either a number (50) or a valid percentage (50%).");

                break;
            case "!quote":
                // Check if a line number has been provided and validate it, otherwise return a random quote from the text file
                if (messageParts.Last() != null)
                {
                    var isNumeric = int.TryParse(messageParts.Last(), out int result);

                    if (isNumeric == false)
                    {
                        Client.SendMessage(e.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
                        return;
                    }

                    // If we got a valid number, do the work
                    var lines = File.ReadAllLines(QuoteCommandMethods.FilePath).Length;

                    if (result < 0 || result >= lines)
                        Client.SendMessage(e.ChatMessage.Channel, $"Whoops! that number is out of range. Try a number from 0 to {lines - 1}");
                    else
                        Client.SendMessage(e.ChatMessage.Channel, QuoteCommandMethods.SelectQuote(result));
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
                break;
            case "!randomquote":
            case "!rq":
                Client.SendMessage(e.ChatMessage.Channel, QuoteCommandMethods.SelectQuote());
                break;
            case "!rngmove":
                if (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator)
                {
                    string move = $"{BotVariables.PIECE_NOTATION[Random.Next(BotVariables.PIECE_NOTATION.Length)]}{BotVariables.FILE_NOTATION[Random.Next(BotVariables.FILE_NOTATION.Length)]}{BotVariables.RANK_NOTATION[Random.Next(BotVariables.RANK_NOTATION.Length)]}";

                    Client.SendMessage(e.ChatMessage.Channel, $"The move for next game is {move} obtoocBri");
                }
                break;
            case "!salute":
                if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    // Get the current volume from the API
                    int originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    int updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, originalVolume);

                    PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundFolder"]}\\Beautiful Trumpet.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");

                break;
            case "!schedule":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's schedule is a complete lie. Just tune in whenever!");
                
                break;
            case "!spotify":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's Spotify page: https://open.spotify.com/user/obtoose");
                break;
            case "!stats":
                if (messageParts.Last() != string.Empty)
                    ChessCommandMethods.GetChesscomStats(Client, e, messageParts.Last());
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !stats (username)");
                break;
            case "!tarf":
                Client.SendMessage(e.ChatMessage.Channel, $"you gotta be bad, you gotta be bold, you gotta be wiser, you gotta be hard, you gotta be tough, you gotta be stronger, you gotta be cool, you gotta be calm, you gotta stay together, all i know love will save the day - corrected");
                break;
            case "!title":
                if (!string.IsNullOrEmpty(messageParts.Last()))
                {
                    TwitchAPI api = new TwitchAPI();

                    // Update the API settings with client ID and OAuth token
                    api.Settings.ClientId = ConfigurationManager.AppSettings["TwitchClientID"];
                    api.Settings.AccessToken = BotVariables.TwitchOAuthToken.Split(":").Last(); // To work with TwitchAPI, the access token can not have the "oauth:" prefix, so we chop it off

                    // Get the broadcaster ID for the channel modification request
                    var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> { BotVariables.ChannelToJoin });
                    string broadcasterId = users.Users[0].Id;

                    await api.Helix.Channels.ModifyChannelInformationAsync(broadcasterId, new ModifyChannelInformationRequest { Title = messageParts.Last() });

                    Client.SendMessage(e.ChatMessage.Channel, $"Title has been updated to \"{messageParts.Last()}\"");
                }

                break;
            case "!twt":
            case "!twitter":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's Twitter: https://twitter.com/oBtuuse");
                break;
            case "!vid":
                Client.SendMessage(e.ChatMessage.Channel, "Latest YouTube video: https://youtu.be/STmFRwBFvqc");
                break;            
            case "!wtf":
                if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                    // Get the current volume from the API
                    int volume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    volume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, volume, 50);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    var currentScene = websocket.GetCurrentProgramScene();
                    var sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var wtfScene = sceneItems.First(item => item.SourceName == "WTF");

                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, wtfScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, volume, 50);
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
                break;
            case "!yt":
            case "!youtube":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's YouTube channel: https://www.youtube.com/channel/UCjS2ciB4D3iftZS3Hj1CCWg");
                break;
            case "f":
                Client.SendMessage(e.ChatMessage.Channel, "obtoocF obtoocF obtoocF obtoocF obtoocF");
                break;
            case "nice":
                Client.SendMessage(e.ChatMessage.Channel, "obtoocNice obtoocNice obtoocNice obtoocNice obtoocNice");
                break;
            case "w":
                Client.SendMessage(e.ChatMessage.Channel, "obtoocW obtoocW obtoocW obtoocW obtoocW");
                break;
            case "!":
                Client.SendMessage(e.ChatMessage.Channel, "obtoocBri obtoocBri obtoocBri obtoocBri obtoocBri");
                break;
            default:
                break;
        }        
    }

    // Need to figure out what to do here in terms of having a timer play
    //private static async Task TimerMessages(CancellationToken cancellationToken = default)
    //{
    //    // Set a message to go off every 30 minutes
    //    using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

    //    while (await timer.WaitForNextTickAsync(cancellationToken))
    //    {
    //        try
    //        {
    //            Client.SendMessage(e.ChatMessage.Channel, "obtoocBri obtoocBri obtoocBri obtoocBri obtoocBri");
    //        }
    //        catch (OperationCanceledException)
    //        {
    //            // Handle cancellation
    //            break;
    //        }
    //    }
    //}
}