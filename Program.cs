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
using System.Data;

class Program
{
    private static TwitchClient Client { get; set; }

    private static SqliteConnection Connection { get; set; } = new SqliteConnection("Data Source=TwitchStats.db");

    public static async Task Main(string[] args)
    {
        // Open the connection to the DB
        Connection.Open();

        // Create all tables that can be used through Twitch chat
        InitializeAllTables();

        // First things 
        HttpClient apiCallClient = new HttpClient()
        {
            BaseAddress = new Uri(BotVariables.OAuthRefreshUri),
        };
                
        // Start out with a newly refreshed token and assign the resulting value to the property in BotVariables for later
        var oauthToken = await RefreshOAuthToken(apiCallClient);
        BotVariables.OAuthToken = oauthToken;

        // Open websocket connection to listen for channel point redemptions
        //WebSocketMethods.StartListeningForEvents();

        // Set up client
        ConnectionCredentials credentials = new ConnectionCredentials(BotVariables.BotUsername, oauthToken);
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

        switch (commandParts.First())
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
            case "!schedule":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's schedule is a complete lie. Just tune in whenever!");
                
                break;
            case "!scam":
                var chatterDisplayName = e.ChatMessage.DisplayName; // Maintains capitalization
                                
                var command = Connection.CreateCommand();
                command.Parameters.AddWithValue("@chatter", chatterDisplayName);

                // Create a new scam record or update an existing one
                command.CommandText = $@"
                    INSERT INTO ScamStatistics (username, scam_count) VALUES (@chatter, 1)
                    ON CONFLICT(username)
                    DO UPDATE SET scam_count = scam_count + 1
                ";

                command.ExecuteNonQuery();

                // Get the relevant scam total from the DB
                command.CommandText = $"SELECT scam_count FROM ScamStatistics WHERE username = @chatter";
                int scamCount = Convert.ToInt32(command.ExecuteScalar());

                // Let 'em know
                Client.SendMessage(e.ChatMessage.Channel, $"{chatterDisplayName}, you've fallen for {scamCount} scams! You really should be more careful...");
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
                    api.Settings.ClientId = BotVariables.ClientID;
                    api.Settings.AccessToken = BotVariables.OAuthToken.Split(":").Last(); // To work with TwitchAPI, the access token can not have the "oauth:" prefix, so we chop it off

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
            case "!yt":
            case "!youtube":
                Client.SendMessage(e.ChatMessage.Channel, "oBtooce's YouTube channel: https://www.youtube.com/channel/UCjS2ciB4D3iftZS3Hj1CCWg");
                break;
            default:
                break;
        }
    }

    private static void InitializeAllTables()
    {
        var command = Connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS ScamStatistics (
                username TEXT PRIMARY KEY,
                scam_count INTEGER DEFAULT 0
            )                    
        ";

        command.ExecuteNonQuery();

        Connection.Close();

        return;
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