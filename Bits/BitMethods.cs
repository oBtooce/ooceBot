using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using ooceBot.AudioVideo;
using ooceBot.Authorization;
using ooceBot.Functionality;
using ooceBot.Sounds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Soundtrack;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;

namespace ooceBot.Bits
{
    public static class BitMethods
    {
        private static ConcurrentQueue<ChatMessage> DecreeQueue = new ConcurrentQueue<ChatMessage>();

        private static bool IsDecreeActive = false;

        private static SemaphoreSlim DecreeLock = new SemaphoreSlim(1, 1);

        private static TwitchClient? Client = null;

        /// <summary>
        /// Method for handling bit donations. Certain bot amounts trigger special events.
        /// </summary>
        /// <param name="client">A TwitchClient instance</param>
        /// <param name="message">A ChatMessage object from the Twitch API</param>
        /// <param name="nightbotClient">A Nightbot client instance</param>
        public static async void HandleBitsMessage(TwitchClient client, ChatMessage message, HttpClient nightbotClient)
        {
            switch (message.Bits)
            {
                case 100:
                    await GiveOutAGoldStar(nightbotClient);
                    break;
                case 250:
                    DecreeQueue.Enqueue(message);

                    // Set up the TwitchClient object for use later
                    if (Client == null)
                        Client = client;

                    await StartNextRoyalDecree();
                    break;
                case 500:
                    ChooseNextStream(message);
                    break;
                default:
                    break;
            }

            // Handle any TTS stuff here

        }

        /// <summary>
        /// A very cool star is handed out to the goodest chatter for a single dollar! Wow!
        /// </summary>
        /// <param name="nightbotClient">A Nightbot client instance</param>
        private static async Task GiveOutAGoldStar(HttpClient nightbotClient)
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

        /// <summary>
        /// Give a bits donator the power to create a "rule" that the streamer must follow (within reason, of course).
        /// Each donation adds the message info to a queue and each message is handled in order.
        /// </summary>
        private static async Task StartNextRoyalDecree()
        {
            OBSWebsocket websocket = await OBSManager.ConnectToOBSWebsocket();

            // Get the lock to determine enable/disable of the queue
            await DecreeLock.WaitAsync();

            try
            {
                // Check for other running decrees and early exit if yes
                if (IsDecreeActive)
                    return;

                // Pop off the first value in the queue and use this for decree determination
                if (DecreeQueue.TryDequeue(out ChatMessage nextInLine))
                {
                    // Check for profanity and deal with it accordingly
                    if (nextInLine.Message.Split(' ').Any(word => BotVariables.BANNED_WORDS.Contains(word)))
                    {
                        // Time out the bad apple
                        string message = $"/timeout {nextInLine.Username} 600 Using profanity is not tolerated. Take a break, yeah?";

                        Client.SendMessage(BotVariables.ChannelToJoin, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(message) : message);
                    }
                    else
                    {
                        IsDecreeActive = true;
                        _ = RunDecreeAsync(nextInLine, websocket);
                    }
                }
                else
                {
                    var settingsObject = new JObject();
                    settingsObject["text"] = "Placeholder";

                    websocket.SetInputSettings("Rule Text", settingsObject);
                }
            }
            finally
            {
                DecreeLock.Release();
            }
        }

        /// <summary>
        /// Creates the new rule and places it on screen for all to see for 5 minutes
        /// </summary>
        /// <param name="message">A ChatMessage object from the Twitch API</param>
        /// <param name="websocket">An OBS websocket instance</param>
        /// <returns></returns>
        private static async Task RunDecreeAsync(ChatMessage message, OBSWebsocket websocket)
        {
            try
            {
                // Announce the decree to chat
                string outputMessage = $"[NEW RULE] {message.Message.Replace("Cheer250", "")}";

                Client.SendMessage(BotVariables.ChannelToJoin, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(outputMessage) : outputMessage);

                var settingsObject = new JObject();
                settingsObject["text"] = message.Message;

                websocket.SetInputSettings("Rule Text", settingsObject);

                // Non-blocking async delay — yields the thread instead of spinning
                await Task.Delay(TimeSpan.FromMinutes(5));

                Client.SendMessage(message.Channel, $"[RULE EXPIRED]");
            }
            finally
            {
                // Always release the active flag, even if something throws
                await DecreeLock.WaitAsync();

                try
                {
                    IsDecreeActive = false;
                }
                finally
                {
                    DecreeLock.Release();
                }

                // Automatically start the next decree if one is waiting
                await StartNextRoyalDecree();
            }
        }

        private static void ChooseNextStream(ChatMessage message)
        {
            string outputMessage = $"{message.DisplayName}, you have been given the power of -----CHOOSER OF CONTENT----- {BotVariables.obtoocBri} What will the next stream be?";

            Client.SendMessage(BotVariables.ChannelToJoin, BotVariables.IsYelling ? StreamCommandFunctionality.MakeItLoud(outputMessage) : outputMessage);
        }
    }
}
