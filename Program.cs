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
using System.Text.Json;
using ooceBot.Commands;

class Program
{
    private static TwitchClient client { get; set; }

    public static async Task Main(string[] args)
    {
        // Configuration
        string botUsername = BotVariables.BotUsername;
        string channelToJoin = BotVariables.ChannelToJoin;

        HttpClient apiCallClient = new HttpClient()
        {
            BaseAddress = new Uri(BotVariables.RefreshUri),
        };

        // Verify token authenticity and refresh if needed
        var oauthToken = await RefreshOAuthToken(apiCallClient);

        // Set up client
        ConnectionCredentials credentials = new ConnectionCredentials(botUsername, oauthToken);
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        var customClient = new WebSocketClient(clientOptions);
        client = new TwitchClient(customClient);
        client.Initialize(credentials);

        client.OnConnected += Client_OnConnected;
        client.OnMessageReceived += Client_OnMessageReceived;

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
        string[] commandParts = e.ChatMessage.Message.Split(new char[] { ' ' }, 2);

        switch (commandParts.First())
        {
            case "!addquote":
                if (commandParts.Last() != string.Empty)
                {
                    QuoteCommandMethods.AddQuote(commandParts.Last());
                    client.SendMessage(e.ChatMessage.Channel, $"Quote added. Thank you for creating history in oBtooce's stream!");
                }
                else
                    client.SendMessage(e.ChatMessage.Channel, $"When using the !addquote command, don't forget to include the quote! The command looks like this: !addquote \"insert quote here\"");
                break;
            case "!audit":
                if (commandParts[1] != null)
                    ChessCommandMethods.AuditChatter(client, e, commandParts.Last());
                else
                    client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !audit (username)");
                break;
            case "!croissant":
                client.SendMessage(e.ChatMessage.Channel, $"https://en.wikipedia.org/wiki/En_passant");
                break;
            case "!dc":
            case "!discord":
                client.SendMessage(e.ChatMessage.Channel, "oBtooce's Discord: https://discord.gg/5RTxTFurGF");
                break;
            case "!f":
                client.SendMessage(e.ChatMessage.Channel, "obtoocF");
                break;
            case "!finecheddar":
                client.SendMessage(e.ChatMessage.Channel, $"https://en.wikipedia.org/wiki/Fianchetto");
                break;
            case "!hello":
                client.SendMessage(e.ChatMessage.Channel, $"Hello {e.ChatMessage.Username}!");
                break;
            case "!lurk":
                client.SendMessage(e.ChatMessage.Channel, $"{e.ChatMessage.Username}, your continued support is greatly appreciated. Talk to you soon!");
                break;
            case "!quote":
                // Check if a line number has been provided and validate it, otherwise return a random quote from the text file
                if (commandParts.Last() != null)
                {
                    var isNumeric = int.TryParse(commandParts.Last(), out int result);

                    if (!isNumeric)
                        client.SendMessage(e.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
                    else
                        client.SendMessage(e.ChatMessage.Channel, QuoteCommandMethods.SelectQuote(result));
                }
                else
                    client.SendMessage(e.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
                break;
            case "!randomquote":
            case "!rq":
                client.SendMessage(e.ChatMessage.Channel, QuoteCommandMethods.SelectQuote());
                break;
            case "!spotify":
                client.SendMessage(e.ChatMessage.Channel, "oBtooce's Spotify page: https://open.spotify.com/user/obtoose");
                break;
            case "!stats":
                if (commandParts.Last() != string.Empty)
                    ChessCommandMethods.GetChesscomStats(client, e, commandParts.Last());
                else
                    client.SendMessage(e.ChatMessage.Channel, $"Hmm...something went wrong. Make sure you are using a valid username and try again with the following format: !stats (username)");
                break;
            case "!twt":
            case "!twitter":
                client.SendMessage(e.ChatMessage.Channel, "oBtooce's Twitter: https://twitter.com/oBtuuse");
                break;
            case "!vid":
                client.SendMessage(e.ChatMessage.Channel, "Latest YouTube video: https://youtu.be/STmFRwBFvqc");
                break;
            case "!yt":
            case "!youtube":
                client.SendMessage(e.ChatMessage.Channel, "oBtooce's YouTube channel: https://www.youtube.com/channel/UCjS2ciB4D3iftZS3Hj1CCWg");
                break;
            default:
                break;
        }
    }

    private static async Task<string> RefreshOAuthToken(HttpClient client)
    {
        // Build the POST content
        var formData = new Dictionary<string, string>
        {
            { "client_id", BotVariables.ClientID },
            { "client_secret", BotVariables.ClientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", BotVariables.OAuthRefreshToken }
        };

        HttpContent requestContent = new FormUrlEncodedContent(formData);

        HttpResponseMessage response = await client.PostAsync(BotVariables.RefreshUri, requestContent);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                JsonElement root = doc.RootElement;

                return $"oauth:{doc.RootElement.GetProperty("access_token")}";
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");

            return string.Empty;
        }
    }
}