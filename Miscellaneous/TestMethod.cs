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
            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            // The source needs to exist in the currently selected scene, so fetch the current scene name and its items
            var currentScene = websocket.GetCurrentProgramScene();
            var sceneItems = websocket.GetSceneItemList(currentScene);

            var thanksScene = sceneItems.First(item => item.SourceName == "Thanks");

            websocket.SetSceneItemEnabled(currentScene, thanksScene.ItemId, true);
            PlaySounds.PlaySound($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\Super_Mario_64_Star_Get_Sound_Effect.mp3");

            await Task.Delay(500);

            PlaySounds.PlaySound($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\SoundEffects\\Here_You_Go.mp3");

            await Task.Delay(1000);

            PlaySounds.PlaySound($"{ConfigurationManager.AppSettings["SoundsFolder"]}\\SoundEffects\\Reverb_Fart.mp3");

            await Task.Delay(600);

            var goldStarScene = sceneItems.First(item => item.SourceName == "Gold Star");
            var confettiScene = sceneItems.First(item => item.SourceName == "Confetti");

            websocket.SetSceneItemEnabled(currentScene, goldStarScene.ItemId, true);
            websocket.SetSceneItemEnabled(currentScene, confettiScene.ItemId, true);

            await Task.Delay(4100);

            websocket.SetSceneItemEnabled(currentScene, thanksScene.ItemId, false);
            websocket.SetSceneItemEnabled(currentScene, goldStarScene.ItemId, false);
            websocket.SetSceneItemEnabled(currentScene, confettiScene.ItemId, false);
        }
    }
}
