using Microsoft.Data.Sqlite;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using ooceBot;
using ooceBot.Functionality;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using OBSWebsocketDotNet;
using ooceBot.Authorization;
using ooceBot.Miscellaneous;
using ooceBot.SQL;
using ooceBot.Sounds;
using ooceBot.AudioVideo;
using System.Configuration;
using ooceBot.Commands;
using TwitchLib.Api.Core;

class Program
{
    private static TwitchClient Client { get; set; }

    private static HttpClient NightbotSongRequestClient { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    private static TwitchAPI _twitchApi = new TwitchAPI(settings: new ApiSettings { ClientId = ConfigurationManager.AppSettings["TwitchClientID"], AccessToken = ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"] });

    public static async Task Main(string[] args)
    {
        // Create all tables that can be used through Twitch chat
        TableSQLMethods.InitializeAllTables(Connection);

        // Set up the command usage table
        DBQueryMethods.PopulateCommandUsageTable(Connection, BotVariables.VideoCommands);

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

        if (BotVariables.CommandsList.TryGetValue(messageParts.First().ToLower(), out BotVariables.Command command))
        {
            if (messageParts.Length == 1)
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), string.Empty));
            else
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), messageParts.Last()));
        }
        else if (BotVariables.VideoCommands.TryGetValue(messageParts.First().ToLower(), out command))
            command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), string.Empty));
    }

    // Need to figure out what to do here in terms of having a timer play
    //private static async Task PostInChat(CancellationToken cancellationToken = default)
    //{
    //    while (!cancellationToken.IsCancellationRequested)
    //    {
    //        Client.SendMessage(args.ChatMessage.Channel, "obtoocBri obtoocBri obtoocBri obtoocBri obtoocBri");
    //    }
    //}
}