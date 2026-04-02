using OBSWebsocketDotNet;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Miscellaneous
{
    public static class TestMethod
    {
        public static async Task TestStuff()
        {
            // Ideas: SM64 star get soundbyte, confetti overlay, 
            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            // The scene needs to exist in the currently selected scene, so fetch the current scene name and its items
            var currentScene = websocket.GetCurrentProgramScene();
            var sceneItems = websocket.GetSceneItemList(currentScene);

            // Need to figure out a way to make the source name not a string because this is a bad setup
            var goldStarScene = sceneItems.First(item => item.SourceName == "Gold Star");

            websocket.SetSceneItemEnabled(currentScene, goldStarScene.ItemId, true);

            PlaySounds.PlaySound($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Super_Mario_64_Star_Get_Sound_Effect.mp3");

            await Task.Delay(5000);

            websocket.SetSceneItemEnabled(currentScene, goldStarScene.ItemId, false);
        }
    }
}
