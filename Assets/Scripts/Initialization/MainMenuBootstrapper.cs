using Services;
using UI.Menu;
using UnityEngine;

namespace Initialization
{
    public sealed class MainMenuBootstrapper : MonoBehaviour
    {
        [SerializeField] SettingsMenu settingsMenu;

        private IAudioService _audioService;

        private void Start()
        {
            _audioService = GameBootstrapper.Instance.Services.AudioService;

            settingsMenu.Initialize(_audioService);
        }
    }
}