using OBSWebsocketDotNet;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using ooceBot.Sounds;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Soundtrack;
using TwitchLib.Client.Models;

namespace ooceBot.Bits
{
    public static class BitMethods
    {
        public static async void HandleBitsMessage(ChatMessage message, HttpClient nightbotClient)
        {
            switch (message.Bits)
            {
                case 100:
                    GiveOutAGoldStar(message, nightbotClient);
                    break;
                case 250:
                    //AssignKingStatus(message);
                    break;
                case 500:
                    break;
                case 1000:
                    break;
                case 5000:
                    break;
                default:
                    break;
            }
        }

        private static async void GiveOutAGoldStar(ChatMessage message, HttpClient nightbotClient)
        {
            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            // Get the current volume from the API
            double originalVolume = await VolumeControl.GetNightbotCurrentVolume(nightbotClient);
            double volumeChange = originalVolume * 0.9;

            // Keep track of the volume for the reset after the video is done
            double updatedVolume = await VolumeControl.ReduceVolume(nightbotClient, originalVolume, volumeChange);

            // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
            var currentScene = websocket.GetCurrentProgramScene();
            var sceneItems = websocket.GetSceneItemList(currentScene);

            // Need to figure out a way to make the source name not a string because this is a bad setup
            var thanksScene = sceneItems.First(item => item.SourceName == "Thanks!");
            var confettiScene = sceneItems.First(item => item.SourceName == "Confetti");

            websocket.SetSceneItemEnabled(currentScene, thanksScene.ItemId, true);

            await Task.Delay(2000);

            websocket.SetSceneItemEnabled(currentScene, confettiScene.ItemId, true);

            websocket.MediaInputPlaybackEnded += async (sender, args) =>
            {
                // Reset the volume after the video is done
                await VolumeControl.IncreaseVolume(nightbotClient, updatedVolume, volumeChange);

                websocket.SetSceneItemEnabled(currentScene, thanksScene.ItemId, false);
                websocket.SetSceneItemEnabled(currentScene, confettiScene.ItemId, false);
            };
        }
    }
}
