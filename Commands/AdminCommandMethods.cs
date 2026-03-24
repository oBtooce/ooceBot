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
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace ooceBot.Commands
{
    public static class AdminCommandMethods
    {
        public static async void Back(CommandArgs args)
        {
            if (args.ChatMessage.IsBroadcaster)
            {
                OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                // Fetch the current scene name and its items
                var currentScene = websocket.GetCurrentProgramScene();
                var sceneItems = websocket.GetSceneItemList(currentScene);

                var brbScreen = sceneItems.First(item => item.SourceName == "We'll Be Right Back");
                websocket.SetSceneItemEnabled(currentScene, brbScreen.ItemId, false);

                websocket.SetSourceFilterEnabled(currentScene, "Freeze Filter", false);
                websocket.SetInputMute("Mic/Aux", false);

                // Get the current volume from the API
                double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);
                double volumeChange = BotVariables.DEFAULT_NIGHTBOT_VOLUME;

                // Keep track of the volume for the reset after the video is done
                double updatedVolume = await VolumeControl.IncreaseVolume(args.NightbotSongRequestClient, originalVolume, volumeChange);
            }
        }

        public static async void BRB(CommandArgs args)
        {
            if (args.ChatMessage.IsBroadcaster)
            {
                OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

                // 3-second delay before screen kicks in
                await Task.Delay(5000);

                // Get the current volume from the API
                double originalVolume = await VolumeControl.GetNightbotCurrentVolume(args.NightbotSongRequestClient);
                double volumeChange = originalVolume;

                // Keep track of the volume for the reset after the video is done
                double updatedVolume = await VolumeControl.ReduceVolume(args.NightbotSongRequestClient, originalVolume, volumeChange);

                // Fetch the current scene name and its items
                var currentScene = websocket.GetCurrentProgramScene();
                var sceneItems = websocket.GetSceneItemList(currentScene);

                var brbScreen = sceneItems.First(item => item.SourceName == "We'll Be Right Back");
                websocket.SetSceneItemEnabled(currentScene, brbScreen.ItemId, true);

                websocket.SetSourceFilterEnabled(currentScene, "Freeze Filter", true);
                websocket.SetInputMute("Mic/Aux", true);

                PlaySounds.PlaySoundWithFader($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\We'll Be Right Back.mp3", 0, 0);
            }
        }

        public static async void Game(CommandArgs args)
        {
            if (args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!string.IsNullOrEmpty(args.CommandQuantifier))
                {
                    var games = await args.TwitchAPI.Helix.Games.GetGamesAsync(gameNames: new List<string> { args.CommandQuantifier });
                    string gameId = games.Games[0].Id;

                    await args.TwitchAPI.Helix.Channels.ModifyChannelInformationAsync(BotVariables.BroadcasterID, new ModifyChannelInformationRequest { GameId = gameId });

                    args.Client.SendMessage(args.ChatMessage.Channel, $"Game has been updated to \"{args.CommandQuantifier}\"");
                }
            }
        }

        public static void RNGMove(CommandArgs args)
        {
            if (args.ChatMessage.IsBroadcaster || args.ChatMessage.IsModerator)
            {
                string move = $"{BotVariables.PIECE_NOTATION[args.Random.Next(BotVariables.PIECE_NOTATION.Length)]}{BotVariables.FILE_NOTATION[args.Random.Next(BotVariables.FILE_NOTATION.Length)]}{BotVariables.RANK_NOTATION[args.Random.Next(BotVariables.RANK_NOTATION.Length)]}";

                args.Client.SendMessage(args.ChatMessage.Channel, $"The move for next game is {move} obtoocBri");
            }
        }

        public static async void Title(CommandArgs args)
        {
            if (args.ChatMessage.IsModerator || args.ChatMessage.IsBroadcaster)
            {
                if (!string.IsNullOrEmpty(args.CommandQuantifier))
                {
                    await args.TwitchAPI.Helix.Channels.ModifyChannelInformationAsync(BotVariables.BroadcasterID, new ModifyChannelInformationRequest { Title = args.CommandQuantifier });

                    args.Client.SendMessage(args.ChatMessage.Channel, $"Title has been updated to \"{args.CommandQuantifier}\"");
                }
            }
        }
    }
}
