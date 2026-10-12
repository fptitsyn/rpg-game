using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu.Pause
{
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject menuPanel;
        [SerializeField] InputActionReference pauseAction;

        public event Action PauseRequested;
        public event Action MainMenuClicked;
        public event Action SaveClicked;
        public event Action LoadClicked;

        private void Update()
        {
            if (pauseAction.action.WasPressedThisFrame()) PauseRequested?.Invoke();
        }

        public void Show(bool visible) => menuPanel.SetActive(visible);

        public void OpenMainMenu() => MainMenuClicked?.Invoke();
        public void Save() => SaveClicked?.Invoke();
        public void Load() => LoadClicked?.Invoke();
    }
}