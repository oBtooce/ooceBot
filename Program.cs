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

class Program
{
    private static TwitchClient Client { get; set; }

    private static HttpClient NightbotSongRequestClient { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    public delegate void Command(CommandArgs args);

    private static Dictionary<string, Command> CommandsList { get; set; } = new Dictionary<string, Command>()
    {
        { "!", CommandMethods.Exclaim },
        { "!addquote", CommandMethods.AddQuote },
        { "!audit", CommandMethods.Audit },
        { "!boner", CommandMethods.Boner },
        { "!buyin", CommandMethods.BuyIn },
        { "!croissant", CommandMethods.Croissant },
        { "!dc", CommandMethods.Discord },
        { "!discord", CommandMethods.Discord },
        { "!emotes", CommandMethods.Emotes },
        { "!finecheddar", CommandMethods.FineCheddar },
        { "!groove", CommandMethods.Groove },
        { "!guts", CommandMethods.Guts },
        { "!help", CommandMethods.Help },
        { "!here", CommandMethods.Here },
        { "!jacob", CommandMethods.Jacob },
        { "!lurk", CommandMethods.Lurk },
        { "!play", CommandMethods.Play },
        { "!quote", CommandMethods.Quote },
        { "!randomquote", CommandMethods.RandomQuote },
        { "!rq", CommandMethods.RandomQuote },
        { "!rngmove", CommandMethods.RNGMove },
        { "!salute", CommandMethods.Salute },
        { "!schedule", CommandMethods.Schedule },
        { "!spotify", CommandMethods.Spotify },
        { "!stats", CommandMethods.Stats },
        { "!tarf", CommandMethods.Tarf },
        { "!title", CommandMethods.Title },
        { "!twitter", CommandMethods.Twitter },
        { "!twt", CommandMethods.Twitter },
        { "!vid", CommandMethods.Vid },
        { "!wtf", CommandMethods.WTF },
        { "!youtube", CommandMethods.YouTube },
        { "!yt", CommandMethods.YouTube },
        { "f", CommandMethods.F },
        { "nice", CommandMethods.Nice },
        { "w", CommandMethods.W }
    };

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

        if (CommandsList.TryGetValue(messageParts.First().ToLower(), out Command command))
        {
            if (messageParts.Length == 1)
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, string.Empty));
            else
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, messageParts.Last()));
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