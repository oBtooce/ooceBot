using OBSWebsocketDotNet;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using ooceBot.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Commands
{
    public static class VideoCommandMethods
    {
        public static async void Lobster(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!BotVariables.IsAudioOrVideoPlaying)
                {
                    OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                    // Get the current volume from the API
                    double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);
                    double volumeChange = originalVolume * 0.9;

                    // Keep track of the volume for the reset after the video is done
                    double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    var currentScene = websocket.GetCurrentProgramScene();
                    var sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var lobsterScene = sceneItems.First(item => item.SourceName == "LOBSTER");

                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, lobsterScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, volumeChange);
                }
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, $"Gotta wait 'til the other stuff is done playing.");
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }

        public static async void WHO(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!BotVariables.IsAudioOrVideoPlaying)
                {
                    OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                    // Get the current volume from the API
                    double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);
                    double volumeChange = originalVolume * 0.9;

                    // Keep track of the volume for the reset after the video is done
                    double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    var currentScene = websocket.GetCurrentProgramScene();
                    var sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var whoScene = sceneItems.First(item => item.SourceName == "WHO");
                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, whoScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, volumeChange);
                }
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, $"Gotta wait 'til the other stuff is done playing.");
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }

        public static async void WTF(CommandArgs args)
        {
            if (args.ChatMessage.IsVip || args.ChatMessage.IsSubscriber || args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!BotVariables.IsAudioOrVideoPlaying)
                {
                    OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                    // Get the current volume from the API
                    double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);
                    double volumeChange = originalVolume * 0.9;

                    // Keep track of the volume for the reset after the video is done
                    double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, volumeChange);

                    // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
                    var currentScene = websocket.GetCurrentProgramScene();
                    var sceneItems = websocket.GetSceneItemList(currentScene);

                    // Need to figure out a way to make the source name not a string because this is a bad setup
                    var wtfScene = sceneItems.First(item => item.SourceName == "WTF");
                    await PlayVideos.PlayVideoAndHideAtEnd(websocket, currentScene, wtfScene);

                    // Reset the volume after the video is done
                    await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, updatedVolume, volumeChange);
                }
                else
                    args.Client.SendMessage(args.ChatMessage.Channel, $"Gotta wait 'til the other stuff is done playing.");
            }
            else
                args.Client.SendMessage(args.ChatMessage.Channel, $"VIPs and subscribers can play song and sound commands. Want in? You know what to do...");
        }
    }
}
