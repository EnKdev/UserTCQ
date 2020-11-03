using UserTCQ.Engine.Types;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace UserTCQ.Engine.Managers
{
    public class AudioSource : IDisposable
    {
        public string name;

        public VorbisWaveReader reader;
        public WaveChannel32 waveChannel;
        public WasapiOut output = new WasapiOut();

        public float volume
        {
            get
            {
                return output.Volume;
            }
            set
            {
                output.Volume = value;
            }
        }

        public AudioSource(string name, string path)
        {
            this.name = name;

            reader = new VorbisWaveReader(path);
            waveChannel = new WaveChannel32(reader);
            output.Init(waveChannel);
        }

        public void Play()
        {
            output.Play();
        }

        public void Stop()
        {
            output.Stop();
        }

        public void Seek(float position)
        {
            waveChannel.Seek((long)position * waveChannel.WaveFormat.SampleRate, System.IO.SeekOrigin.Begin);
        }

        public float GetPosition()
        {
            return (float)waveChannel.CurrentTime.TotalSeconds;
        }

        public void Dispose()
        {
            AudioManager.audioSources.Remove(name);

            output.Dispose();
            waveChannel.Dispose();
            reader.Dispose();
        }
    }

    public class AudioManager : Behaviour
    {
        public static Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

        public static AudioSource AddAudioSource(string name, string path)
        {
            var audioSource = new AudioSource(name, path);
            audioSources.Add(name, audioSource);
            return audioSource;
        }

        public static void RemoveAudioSource(string name)
        {
            audioSources[name].Dispose();
            audioSources.Remove(name);
        }
    }
}
