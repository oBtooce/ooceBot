using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using ooceBot.AudioVideo;
using ooceBot.Commands;
using ooceBot.Sounds;
using System.Configuration;
using System.Net.WebSockets;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.PubSub.Events;

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

            switch (channelPointReward.Reward.Title) 
            {
                case "Something To Make You Smile :)":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    await PlayVideoInOBS(websocket, originalVolume, volumeChange, "Maggie");
                    break;
                case "The Cure For Sadness...":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    await PlayVideoInOBS(websocket, originalVolume, volumeChange, "Homer");
                    break;
                case "Lobster":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    await PlayVideoInOBS(websocket, originalVolume, volumeChange, "LOBSTER");
                    break;
                case "Who Do You Think You Are!?":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    await PlayVideoInOBS(websocket, originalVolume, volumeChange, "WHO");
                    break;
                case "WTF":
                    // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
                    originalVolume = await VolumeControl.GetNightbotCurrentVolume(NightbotSongRequestClient);
                    volumeChange = (int)(originalVolume * 0.9);

                    await PlayVideoInOBS(websocket, originalVolume, volumeChange, "WTF");
                    break;
                default:
                    break;
            }
        }

        private static async Task PlayVideoInOBS(OBSWebsocket websocket, int originalVolume, int volumeChange, string sceneName)
        {
            // Keep track of the volume for the reset after the video is done
            int updatedVolume = await VolumeControl.ReduceVolume(NightbotSongRequestClient, originalVolume, volumeChange);

            // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
            string currentScene = websocket.GetCurrentProgramScene();
            List<SceneItemDetails> sceneItems = websocket.GetSceneItemList(currentScene);

            var scene = sceneItems.First(item => item.SourceName == sceneName);

            await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, scene);

            // Reset the volume after the video is done
            await VolumeControl.IncreaseVolume(NightbotSongRequestClient, updatedVolume, volumeChange);

            return;
        }
    }
}
