using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using ooceBot.Sounds;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace ooceBot.Miscellaneous
{
    public static class TestMethod
    {
        public static async Task TestStuff(ChatMessage message, HttpClient nightbotClient)
        {
            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            // Get the current volume from the API (whole numbers are the only accepted values, so we use ints for all calculations)
            int originalVolume = await VolumeControl.GetNightbotCurrentVolume(nightbotClient);
            int volumeChange = (int)(originalVolume * 0.9);

            // Keep track of the volume for the reset after the video is done
            int updatedVolume = await VolumeControl.ReduceVolume(nightbotClient, originalVolume, volumeChange);

            // The source needs to exist in the currently selected scene, so fetch the current scene name and its items
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
