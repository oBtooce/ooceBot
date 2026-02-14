using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Authorization
{
    public static class OBSManager
    {
        public static async Task<OBSWebsocket> ConnectToOBSWebsocket()
        {
            OBSWebsocket websocket = new OBSWebsocket();

            // Since the websocket's ConnectAsync method does not return a Task, we perform the Task functionality around the method
            var isConnectionEstablished = new TaskCompletionSource<bool>();

            websocket.Connected += (sender, e) =>
            {
                isConnectionEstablished.SetResult(true);
            };

            websocket.ConnectAsync(ConfigurationManager.AppSettings["OBSWebsocketAddress"], ConfigurationManager.AppSettings["OBSWebsocketPassword"]);

            // This await is where the connection is confirmed
            await isConnectionEstablished.Task;

            return websocket;
        }
    }
}
