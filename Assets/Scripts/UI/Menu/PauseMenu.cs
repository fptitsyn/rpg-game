using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject menuPanel;
        [SerializeField] InputActionReference pauseAction;

        private bool _isPaused;

        public event Action MainMenuClicked;

        private void Awake()
        {
            menuPanel.SetActive(false);
        }

        private void OnEnable()
        {
            pauseAction.action.performed += ToggleMenu;
            pauseAction.action.Enable();
        }

        private void OnDisable()
        {
            pauseAction.action.performed -= ToggleMenu;
            pauseAction.action.Disable();

            Time.timeScale = 1f;
        }

        public void OpenMainMenu()
        {
            SetPaused(false);
            MainMenuClicked?.Invoke();
        }

        public void Resume()
        {
            SetPaused(false);
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            SetPaused(!_isPaused);
        }

        private void SetPaused(bool value)
        {
            _isPaused = value;
            menuPanel.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0f : 1f;

            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
        }
    }
}