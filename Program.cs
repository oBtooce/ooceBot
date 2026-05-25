using Microsoft.Data.Sqlite;
using ooceBot;
using ooceBot.Authorization;
using ooceBot.Bits;
using ooceBot.Commands;
using ooceBot.Miscellaneous;
using ooceBot.Models;
using ooceBot.SQL;
using ooceBot.Timers;
using System;
using System.Configuration;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.EventSub.Websockets;

class Program
{
    public static TwitchClient Client { get; set; }

    public static HttpClient NightbotSongRequestClient { get; set; }

    public static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    public static TwitchAPI Api { get; set; } = new TwitchAPI();

    public static TwitchBotContext dbContext = new TwitchBotContext();

    public static async Task Main(string[] args)
    {
        // Create all tables that can be used through Twitch chat
        TableSQLMethods.InitializeAllTables(Connection);

        NightbotSongRequestClient = Startup.SetupNightbotClient();
        Client = Startup.SetupTwitchClient();
        await Startup.InitializeBroadcasterAccessToken();
        Api = new TwitchAPI(settings: new ApiSettings { ClientId = ConfigurationManager.AppSettings["TwitchClientID"], AccessToken = ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"] });

        // Set up all variables that require Twitch Helix
        Startup.UpdateHelixVariables(Api);

        Client.OnConnected += Client_OnConnected;
        Client.OnMessageReceived += Client_OnMessageReceived;
        Client.OnError += (s, e) => Console.WriteLine("Error: " + e.Exception.Message);
        Client.OnLog += (s, e) =>
        {
            Console.WriteLine($"{e.DateTime:HH:mm:ss} {e.BotUsername} - {e.Data}");
        };

        Client.Connect();

        await EventSubWebsocketManager.SetupEventSub(NightbotSongRequestClient, Api);

        Console.ReadLine();
    }

    private static void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Client.JoinChannel(BotVariables.ChannelToJoin);

        _ = TimerMethods.PostMessageInChat(Client, TimeSpan.FromMinutes(10));
    }

    private static async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string[] messageParts = e.ChatMessage.Message.Split(new char[] { ' ' }, 2);

        DBQueryMethods.UpdateChatterDataPlusMaybeTheme(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, Api, messageParts.First(), string.Empty, dbContext));

        // Open new connection
        Connection.Open();

        //await TestMethod.TestStuff(e.ChatMessage, NightbotSongRequestClient);

        // Check for bits first, then check for commands, and also make sure to ignore commands when bits are used
        if (e.ChatMessage.Bits > 0)
            BitMethods.HandleBitsMessage(Client, e.ChatMessage, NightbotSongRequestClient);
        else if (BotVariables.CommandsList.TryGetValue(messageParts.First().ToLower(), out BotVariables.Command command) || BotVariables.AdminCommands.TryGetValue(messageParts.First().ToLower(), out command))
            if (messageParts.Length == 1)
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, Api, messageParts.First(), string.Empty, dbContext));
            else
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, Api, messageParts.First(), messageParts.Last(), dbContext));
        else if (BotVariables.VideoCommands.TryGetValue(messageParts.First().ToLower(), out command) || BotVariables.WordCommands.TryGetValue(messageParts.First().ToLower(), out command))
            command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, Api, messageParts.First(), string.Empty, dbContext));
    }
}