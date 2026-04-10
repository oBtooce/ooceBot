using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using Microsoft.Extensions.Logging;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using Microsoft.Extensions.Hosting;
using TwitchLib.PubSub.Events;
using ooceBot.Commands;
using OBSWebsocketDotNet;
using ooceBot.AudioVideo;
using ooceBot.Sounds;
using TwitchLib.Api.Core.Enums;
using System.Configuration;
using TwitchLib.Api;
using OBSWebsocketDotNet.Types;

namespace ooceBot.Authorization
{
    public class EventSubWebsocketManager
    {
        public static EventSubWebsocketClient _eventSubClient { get; set; }

        public static HttpClient NightbotSongRequestClient { get; set; }

        public static TwitchAPI _twitchApi { get; set; }

        public static async Task SetupEventSub(HttpClient nightbotClient, TwitchAPI api)
        {
            _eventSubClient = new EventSubWebsocketClient();
            NightbotSongRequestClient = nightbotClient;
            _twitchApi = api;

            _eventSubClient.WebsocketConnected += OnWebsocketConnected;
            _eventSubClient.WebsocketDisconnected += OnWebsocketDisconnected;
            _eventSubClient.ChannelPointsCustomRewardRedemptionAdd += OnRedemption;

            await _eventSubClient.ConnectAsync();
        }

        public static async Task OnWebsocketConnected(object sender, WebsocketConnectedArgs e)
        {
            if (!e.IsRequestedReconnect)
            {
                // Subscribe to channel point redemptions via the Twitch API
                await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                    "channel.channel_points_custom_reward_redemption.add",
                    "1",
                    new Dictionary<string, string>
                    {
                        { "broadcaster_user_id", BotVariables.BroadcasterID }
                    },
                    EventSubTransportMethod.Websocket,
                    _eventSubClient.SessionId
                );
            }
        }

        public static async Task OnWebsocketDisconnected(object sender, WebsocketDisconnectedArgs e)
        {
            while (!await _eventSubClient.ReconnectAsync())
            {
                Console.WriteLine("Websocket reconnect failed, retrying...");
                await Task.Delay(1000);
            }
        }

        public static async Task OnRedemption(object sender, ChannelPointsCustomRewardRedemptionArgs e)
        {
            var channelPointReward = e.Payload.Event;

            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            int originalVolume;
            int volumeChange;
            int updatedVolume;

            string currentScene;
            List<SceneItemDetails> sceneItems;

            switch (channelPointReward.Reward.Title) 
            {
                case "Lobster":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    // Keep track of the volume for the reset after the video is done
                    updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    currentScene = websocket.GetCurrentProgramScene();
                    sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var lobsterScene = sceneItems.First(item => item.SourceName == "LOBSTER");

                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, lobsterScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, volumeChange);
                    break;
                case "Who Do You Think You Are!?":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    // Keep track of the volume for the reset after the video is done
                    updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    currentScene = websocket.GetCurrentProgramScene();
                    sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var whoScene = sceneItems.First(item => item.SourceName == "WHO");
                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, whoScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, volumeChange);
                    break;
                case "WTF":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    // Keep track of the volume for the reset after the video is done
                    updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    currentScene = websocket.GetCurrentProgramScene();
                    sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var wtfScene = sceneItems.First(item => item.SourceName == "WTF");
                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, wtfScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, volumeChange);
                    break;
                default:
                    break;
            }
        }
    }
}
