using Microsoft.VisualBasic;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using ooceBot;
using System.Net.Http;

class Program
{
    private static TwitchClient client;

    static void Main(string[] args)
    {
        // Configuration
        string botUsername = BotVariables.BotUsername;
        string oauthToken = BotVariables.OAuthToken;
        string channelToJoin = BotVariables.ChannelToJoin;

        HttpClient apiCallClient = new HttpClient()
        {
            BaseAddress = new Uri(BotVariables.RefreshUri),
        };

        // Verify token authenticity and refresh if needed
        RefreshOAuthToken(BotVariables.OAuthRefreshToken, apiCallClient);

        // Set up client
        ConnectionCredentials credentials = new ConnectionCredentials(botUsername, oauthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        var customClient = new WebSocketClient(clientOptions);
        var client = new TwitchClient(customClient);
        client.Initialize(credentials);

        client.OnConnected += Client_OnConnected;

        client.OnFailureToReceiveJoinConfirmation += (s, e) =>
        {
            Console.WriteLine("Failed to join channel!");
        };

        client.OnError += (s, e) => Console.WriteLine("Error: " + e.Exception.Message);

        client.OnLog += (s, e) =>
        {
            Console.WriteLine($"{e.DateTime:HH:mm:ss} {e.BotUsername} - {e.Data}");
        };

        client.Connect();
        Console.ReadLine();
    }

    private static void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine($"Connected to Twitch as {e.BotUsername}");
        client.JoinChannel(BotVariables.ChannelToJoin);
    }

    private static void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        // Send "Hello world" message when joining
        client.SendMessage(e.Channel, "ooceBot is in the house!");
    }

    private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        switch (e.ChatMessage.Message.ToLower())
        {
            case "!hello":
                client.SendMessage(e.ChatMessage.Channel, $"Hello {e.ChatMessage.Username}!");
                break;
            case "!lurk":
                client.SendMessage(e.ChatMessage.Channel, $"{e.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon!");
                break;
            default:
                break;
        }
    }

    private void RefreshOAuthToken(HttpClient client)
    {
        // Build the POST content
        string jsonData = "{ \"client_id\", \"" +  + "\" }";
        HttpContent requestContent = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
    }
}