using UnityEngine;
using UnityEngine.Audio;

namespace Services
{
    public interface IAudioService
    {
        float MusicVolume { get; }
        float SfxVolume { get; }

        void SetMusicVolume(float value);
        void SetSfxVolume(float value);
    }

    public sealed class AudioService : IAudioService
    {
        private const string MusicParameter = "MusicVolume";
        private const string SfxParameter = "SfxVolume";

        private const string MusicKey = "MusicVolume";
        private const string SfxKey = "SfxVolume";

        private readonly AudioMixer _audioMixer;

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }

        public AudioService(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;

            MusicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, 1f);

            ApplyVolume(MusicParameter, MusicVolume);
            ApplyVolume(SfxParameter, SfxVolume);
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = value;
            ApplyVolume(MusicParameter, MusicVolume);
            PlayerPrefs.SetFloat(MusicKey, MusicVolume);

            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = value;
            ApplyVolume(SfxParameter, SfxVolume);
            PlayerPrefs.SetFloat(SfxKey, SfxVolume);

            PlayerPrefs.Save();
        }

        private void ApplyVolume(string parameter, float value)
        {
            float decibels = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            _audioMixer.SetFloat(parameter, decibels);
        }
    }
}