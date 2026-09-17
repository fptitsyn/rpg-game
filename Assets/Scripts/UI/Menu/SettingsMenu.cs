using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public sealed class SettingsMenu : MonoBehaviour
    {
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider sfxSlider;

        private IAudioService _audioService;
        
        public void Initialize(IAudioService audioService)
        {
            _audioService = audioService;

            musicSlider.SetValueWithoutNotify(_audioService.MusicVolume);
            sfxSlider.SetValueWithoutNotify(_audioService.SfxVolume);
            
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        private void SetMusicVolume(float value)
        {
            _audioService.SetMusicVolume(value);
        }

        private void SetSfxVolume(float value)
        {
            _audioService.SetSfxVolume(value);
        }

        private void OnDestroy()
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }
    }
}