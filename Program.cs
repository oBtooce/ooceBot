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
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;

class Program
{
    private static TwitchClient client { get; set; }

    public static async Task Main(string[] args)
    {
        // Set up HTTP client object with OAuth refresh URI as the base address
        HttpClient apiCallClient = new HttpClient()
        {
            BaseAddress = new Uri(BotVariables.OAuthRefreshUri),
        };

        // Verify token authenticity and refresh if needed
        var oauthToken = await RefreshOAuthToken(apiCallClient);

        // Open websocket connection to listen for channel point redemptions
        //☻WebSocketMethods.StartListeningForEvents();

        // Set up client
        ConnectionCredentials credentials = new ConnectionCredentials(BotVariables.BotUsername, oauthToken);
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
        client.JoinChannel(BotVariables.ChannelToJoin);
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
            //case "!commands":
            //    string commandList = BuildCommandList();
            //    client.SendMessage(e.ChatMessage.Channel, $"Here is the list of usable commands in this stream: {commandList}");
            //    break;
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

                    if (isNumeric == false)
                    {
                        client.SendMessage(e.ChatMessage.Channel, "If you are choosing a quote, make sure you enter a number! Otherwise, just type !randomquote or !rq for a random quote.");
                        return;
                    }

                    // If we got a valid number, do the work
                    var lines = File.ReadAllLines(QuoteCommandMethods.FilePath).Length;

                    if (result < 0 || result >= lines)
                        client.SendMessage(e.ChatMessage.Channel, $"Whoops! that number is out of range. Try a number from 0 to {lines - 1}");
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
            case "!schedule":
                client.SendMessage(e.ChatMessage.Channel, "oBtooce's schedule: Friday 7 PM EST | Saturday 7 PM EST | Sunday 7 PM EST");
                break;
            //case "!scam":
            //    client.SendMessage(e.ChatMessage.Channel, GeneralCommandMethods.);
            //    break;
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

        HttpResponseMessage response = await client.PostAsync(BotVariables.OAuthRefreshUri, requestContent);

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