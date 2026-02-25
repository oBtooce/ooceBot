using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace ooceBot.AudioVideo
{
    public static class PlaySounds
    {
        /// <summary>
        /// Plays a sound from a file with no other effects.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        public static void PlaySound(string filePath)
        {
            // Set up all required values to play a sound (reader and WaveOutEvent for playing audio)
            AudioFileReader audioReader = new AudioFileReader(filePath);
            WaveOutEvent track = new WaveOutEvent();

            // Let 'er rip
            track.Init(audioReader);
            track.Play();
        }

        /// <summary>
        /// Plays a sound from a file with specified fade effects.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="fadeInTime">The specified fade-in time</param>
        /// <param name="fadeOutTime">The specified fade-out time</param>
        public static void PlaySoundWithFader(string filePath, int fadeInTime, int fadeOutTime)
        {
            // Set up all required values to play a sound (reader, fader, and WaveOutEvent for playing audio)
            AudioFileReader audioReader = new AudioFileReader(filePath);
            FadeInOutSampleProvider fader = new FadeInOutSampleProvider(audioReader, true);

            bool isTrackFading = false;

            fader.BeginFadeIn(fadeInTime);

            WaveOutEvent track = new WaveOutEvent();
            track.Init(fader);
            track.Play();

            // Update check for video and audio commands to prevent overlap
            BotVariables.IsAudioOrVideoPlaying = true;

            while (track.PlaybackState == PlaybackState.Playing)
            {
                double songLength = audioReader.TotalTime.TotalMilliseconds;
                double currentTime = audioReader.CurrentTime.TotalMilliseconds;

                var fadeOutSpot = songLength - fadeOutTime;

                // When the song reaches the fade out spot, do the thing
                if (currentTime >= fadeOutSpot && !isTrackFading)
                {
                    fader.BeginFadeOut(fadeOutTime);
                    isTrackFading = true;
                }
            }

            // Allow video and audio commands again
            BotVariables.IsAudioOrVideoPlaying = false;
        }
    }
}
