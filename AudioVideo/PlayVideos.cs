using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.AudioVideo
{
    public static class PlayVideos
    {
        /// <summary>
        /// Plays a media source in OBS, then manually hides the source when done. The manual hiding is required to prevent the source from auto-playing on future scene selection.
        /// </summary>
        /// <param name="websocket">An OBS websocket object</param>
        /// <param name="currentScene">The scene that contains the media source</param>
        /// <param name="source">A media source object</param>
        public static async Task PlayVideoAndHideAtEnd(OBSWebsocket websocket, string currentScene, SceneItemDetails source)
        {
            websocket.TriggerMediaInputAction(source.SourceName, "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_RESTART");

            await Task.Delay(100);

            // Get the duration of the video so that the source can be properly hidden after playback
            // (for some reason, after the media is done playing, the source is still shown as enabled)
            var mediaStatus = websocket.GetMediaInputStatus(source.SourceName);
            var videoLength = mediaStatus.Duration;

            // Update check for video and audio commands to prevent overlap
            BotVariables.IsAudioOrVideoPlaying = true;

            // Enable, then disable after the clip is done (media needs to be active so we can retrieve the duration)
            websocket.SetSceneItemEnabled(currentScene, source.ItemId, true);

            await Task.Delay((int)videoLength - 100);

            // Now that the video has ended, disable the source
            websocket.SetSceneItemEnabled(currentScene, source.ItemId, false);

            // Allow video and audio commands again
            BotVariables.IsAudioOrVideoPlaying = false;

            return;
        }
    }
}
