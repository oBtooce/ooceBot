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

class Program
{
    private static TwitchClient Client { get; set; }

    private static HttpClient NightbotSongRequestClient { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    private static Random Random { get; set; } = new Random();

    public static async Task Main(string[] args)
    {
        // Open the connection to the DB
        Connection.Open();

        // Create all tables that can be used through Twitch chat
        InitializeAllTables();

        // Set access tokens for Nightbot and Twitch
        //await NighbotOAuthManager.SetNightbotOAuthToken();
        await TwitchOAuthManager.SetTwitchOAuthToken();

        // Set up Nightbot song requests client
        NightbotSongRequestClient = new HttpClient()
        {
            BaseAddress = new Uri(BotVariables.NightbotApiRequestUriValue)
        };

        NightbotSongRequestClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BotVariables.NightbotOAuthToken);

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
        string[] commandParts = e.ChatMessage.Message.Split(new char[] { ' ' }, 2);

        // Open new connection
        Connection.Open();

        switch (commandParts.First().ToLower())
        {
            case "!addquote":
                if (commandParts.Last() != string.Empty)
                {
                    QuoteCommandMethods.AddQuote(commandParts.Last());
                    Client.SendMessage(e.ChatMessage.Channel, $"Quote added. Thank you for creating history in oBtooce's stream!");
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"When using the !addquote command, don't forget to include the quote! The command looks like this: !addquote \"insert quote here\"");
                break;
            case "!audit":
                if (commandParts[1] != null)
                    ChessCommandMethods.AuditChatter(Client, e, commandParts.Last());
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !audit (username)");
                break;
            case "!boner":
                Client.SendMessage(e.ChatMessage.Channel, $"don't get married");
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
            case "!guts":
                if (e.ChatMessage.IsVip || e.ChatMessage.IsSubscriber || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    // Get the current volume from the API
                    int originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    int updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, originalVolume);

                    PlaySoundWithFader($"{BotVariables.SoundFolder}\\Berserk soundtrack - 4 Gatsu.mp3", 2000, 2000);

                    // Reset the volume to its previous level
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, originalVolume);
                }
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");

                break;
            case "!here":
                var command = Connection.CreateCommand();

                var chatterDisplayName = e.ChatMessage.DisplayName; // Maintains capitalization
                command.Parameters.AddWithValue("@chatter", chatterDisplayName);

                // Initial check to see if user has declared their presence today
                command.CommandText = $"SELECT is_present FROM ChatterStatistics WHERE username = @chatter";
                var attendanceTakenValue = command.ExecuteScalar();

                // If the user exists but attendance was already taken, then prevent it from happening again
                if (attendanceTakenValue != null && (long)attendanceTakenValue == 1)
                {
                    Client.SendMessage(e.ChatMessage.Channel, "Your attendance has already been taken. Check in next time obtoocBri");
                    return;
                }

                // Create a new attendance record or update an existing one
                command.CommandText = $@"
                    INSERT INTO ChatterStatistics (username, attendance_count, is_present) VALUES (@chatter, 1, 1)
                    ON CONFLICT(username)
                    DO UPDATE SET attendance_count = attendance_count + 1, is_present = 1
                ";

                command.ExecuteNonQuery();

                // Get the relevant attendance total from the DB
                command.CommandText = $"SELECT attendance_count FROM ChatterStatistics WHERE username = @chatter";
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
                Client.SendMessage(e.ChatMessage.Channel, $"{e.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon!");
                break;
            case "!quote":
                // Check if a line number has been provided and validate it, otherwise return a random quote from the text file
                if (commandParts.Last() != null)
                {
                    var isNumeric = int.TryParse(commandParts.Last(), out int result);

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

                    PlaySoundWithFader($"{BotVariables.SoundFolder}\\Beautiful Trumpet.mp3", 2000, 2000);

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
                if (commandParts.Last() != string.Empty)
                    ChessCommandMethods.GetChesscomStats(Client, e, commandParts.Last());
                else
                    Client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !stats (username)");
                break;
            case "!title":
                if (!string.IsNullOrEmpty(commandParts.Last()))
                {
                    TwitchAPI api = new TwitchAPI();

                    // Update the API settings with client ID and OAuth token
                    api.Settings.ClientId = BotVariables.TwitchClientID;
                    api.Settings.AccessToken = BotVariables.TwitchOAuthToken.Split(":").Last(); // To work with TwitchAPI, the access token can not have the "oauth:" prefix, so we chop it off

                    // Get the broadcaster ID for the channel modification request
                    var users = await api.Helix.Users.GetUsersAsync(logins: new List<string> { BotVariables.ChannelToJoin });
                    string broadcasterId = users.Users[0].Id;

                    await api.Helix.Channels.ModifyChannelInformationAsync(broadcasterId, new ModifyChannelInformationRequest { Title = commandParts.Last() });

                    Client.SendMessage(e.ChatMessage.Channel, $"Title has been updated to \"{commandParts.Last()}\"");
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
                    OBSWebsocket websocket = await ConnectToOBSWebsocket();

                    // Get the current volume from the API
                    int volume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);

                    // Keep track of the volume for the reset after the video is done
                    volume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, volume, 50);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    var currentScene = websocket.GetCurrentProgramScene();
                    var sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var wtfScene = sceneItems.First(item => item.SourceName == "WTF");

                    await PlayVideoAndHideAtEnd(websocket, currentScene, wtfScene);

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
            case "nice":
                Client.SendMessage(e.ChatMessage.Channel, "obtoocNice");
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

    private static async Task<OBSWebsocket> ConnectToOBSWebsocket()
    {
        OBSWebsocket websocket = new OBSWebsocket();

        // Since the websocket's ConnectAsync method does not return a Task, we perform the Task functionality around the method
        var isConnectionEstablished = new TaskCompletionSource<bool>();

        websocket.Connected += (sender, e) =>
        {
            isConnectionEstablished.SetResult(true);
        };

        websocket.ConnectAsync(BotVariables.OBSWebsocketAddress, BotVariables.OBSWebsocketPassword);

        // This await is where the connection is confirmed
        await isConnectionEstablished.Task;

        return websocket;
    }

    /// <summary>
    /// Creates all required tables for stream.
    /// </summary>
    private static void InitializeAllTables()
    {
        var command = Connection.CreateCommand();

        // Table creation
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS ChatterStatistics (
                username TEXT PRIMARY KEY,
                attendance_count INTEGER DEFAULT 0,
                is_present INTEGER NOT NULL DEFAULT 0
            )
        ";
        command.ExecuteNonQuery();

        // Attendance is reset for the day
        command.CommandText = "UPDATE ChatterStatistics SET is_present = 0";
        command.ExecuteNonQuery();

        Connection.Close();

        return;
    }

    /// <summary>
    /// Plays a sound from a file with no other effects.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    private static void PlaySound(string filePath)
    {
        // Set up all required values to play a sound (reader and WaveOutEvent for playing audio)
        AudioFileReader audioReader = new AudioFileReader(filePath);
        WaveOutEvent track = new WaveOutEvent();

        // Let 'er rip
        track.Init(audioReader);
        track.Play();
    }

    /// <summary>
    /// Plays a sound from a file with specified fade effects.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <param name="fadeInTime">The specified fade-in time</param>
    /// <param name="fadeOutTime">The specified fade-out time</param>
    private static void PlaySoundWithFader(string filePath, int fadeInTime, int fadeOutTime)
    {
        // Set up all required values to play a sound (reader, fader, and WaveOutEvent for playing audio)
        AudioFileReader audioReader = new AudioFileReader(filePath);
        FadeInOutSampleProvider fader = new FadeInOutSampleProvider(audioReader, true);

        bool isTrackFading = false;

        fader.BeginFadeIn(fadeInTime);

        WaveOutEvent track = new WaveOutEvent();
        track.Init(fader);
        track.Play();
        
        while (track.PlaybackState == PlaybackState.Playing)
        {
            double songLength = audioReader.TotalTime.TotalMilliseconds;
            double currentTime = audioReader.CurrentTime.TotalMilliseconds;

            var fadeOutSpot = songLength - fadeOutTime;

            // When the song reaches the fade out spot, do the thing
            if (currentTime >= fadeOutSpot && !isTrackFading)
            {
                fader.BeginFadeOut(fadeOutTime);
                isTrackFading = true;
            }
        }
    }

    /// <summary>
    /// Plays a media source in OBS, then manually hides the source when done. The manual hiding is required to prevent the source from auto-playing on future scene selection.
    /// </summary>
    /// <param name="websocket">An OBS websocket object</param>
    /// <param name="currentScene">The scene that contains the media source</param>
    /// <param name="source">A media source object</param>
    private static async Task PlayVideoAndHideAtEnd(OBSWebsocket websocket, string currentScene, SceneItemDetails source)
    {
        websocket.TriggerMediaInputAction(source.SourceName, "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_RESTART");

        await Task.Delay(100);

        // Get the duration of the video so that the source can be properly hidden after playback
        // (for some reason, after the media is done playing, the source is still shown as enabled)
        var mediaStatus = websocket.GetMediaInputStatus(source.SourceName);
        var videoLength = mediaStatus.Duration;

        // Enable, then disable after the clip is done (media needs to be active so we can retrieve the duration)
        websocket.SetSceneItemEnabled(currentScene, source.ItemId, true);

        await Task.Delay((int)videoLength - 100);

        // Now that the video has ended, disable the source
        websocket.SetSceneItemEnabled(currentScene, source.ItemId, false);

        return;
    }
}