using Microsoft.Data.Sqlite;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using ooceBot;
using ooceBot.Authorization;
using ooceBot.SQL;
using System.Configuration;
using ooceBot.Commands;
using TwitchLib.Api.Core;
using ooceBot.Timers;
using ooceBot.Bits;
using ooceBot.Miscellaneous;

class Program
{
    public static TwitchClient Client { get; set; }

    private static HttpClient NightbotSongRequestClient { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    public static TwitchAPI _twitchApi = new TwitchAPI(settings: new ApiSettings { ClientId = ConfigurationManager.AppSettings["TwitchClientID"], AccessToken = ConfigurationManager.AppSettings["TwitchBroadcasterAccessToken"] });

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

        // Get the broadcaster ID for later use
        var users = await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> { BotVariables.ChannelToJoin });
        BotVariables.BroadcasterID = users.Users[0].Id;

        // Get all currently enabled custom rewards
        var customRewards = await _twitchApi.Helix.ChannelPoints.GetCustomRewardAsync(BotVariables.BroadcasterID);

        // Add each value to the dictionary
        var redeemData = customRewards.Data.Where(reward => reward.IsEnabled).OrderBy(reward => reward.Cost).ToList();

        // Setting start value to 1 to make it easier for user selection
        for (int i = 0; i < redeemData.Count; i++)
            BotVariables.CustomRewards.Add(i + 1, redeemData[i]);

        await GistManager.UpdateRewardsGist(BotVariables.CustomRewards);

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

        _ = TimerMethods.PostMessageInChat(Client, TimeSpan.FromMinutes(10));
    }

    private static async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        DBQueryMethods.UpdateChatterDataPlusMaybeTheme(Connection, e.ChatMessage);

        string[] messageParts = e.ChatMessage.Message.Split(new char[] { ' ' }, 2);

        // Open new connection
        Connection.Open();

        //await TestMethod.TestStuff(e.ChatMessage, NightbotSongRequestClient);

        // Check for bits first, then check for commands, and also make sure to ignore commands when bits are used
        if (e.ChatMessage.Bits > 0)
            BitMethods.HandleBitsMessage(Client, e.ChatMessage, NightbotSongRequestClient);
        else if (BotVariables.CommandsList.TryGetValue(messageParts.First().ToLower(), out BotVariables.Command command) || BotVariables.AdminCommands.TryGetValue(messageParts.First().ToLower(), out command))
            if (messageParts.Length == 1)
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), string.Empty));
            else
                command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), messageParts.Last()));
        else if (BotVariables.VideoCommands.TryGetValue(messageParts.First().ToLower(), out command) || BotVariables.WordCommands.TryGetValue(messageParts.First().ToLower(), out command))
            command(new CommandArgs(Client, e.ChatMessage, Connection, NightbotSongRequestClient, _twitchApi, messageParts.First(), string.Empty));
    }
}