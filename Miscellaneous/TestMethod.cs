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

            var settingsObject = new JObject();
            settingsObject["text"] = message.Message;

            websocket.SetInputSettings("Rule Text", settingsObject, overlay: true);
        }
    }
}
