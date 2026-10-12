using System;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Pause
{
    public class PauseMenuController : IDisposable
    {
        private readonly PauseMenuModel _model;
        private readonly PauseMenu _view;
        private readonly GameSaveInteractor _saveInteractor;
        private readonly Action _openMainMenu;

        public PauseMenuController(PauseMenuModel model, PauseMenu view, GameSaveInteractor saveInteractor,
            Action openMainMenu)
        {
            _model = model;
            _view = view;
            _saveInteractor = saveInteractor;
            _openMainMenu = openMainMenu;

            _view.PauseRequested += Toggle;
            _view.MainMenuClicked += OpenMainMenu;
            _view.SaveClicked += Save;
            _view.LoadClicked += Load;
            SetOpen(false);
        }

        private void Toggle() => SetOpen(!_model.IsOpen);
        
        private void Save() => _saveInteractor.Save();

        private void Load()
        {
            if (_saveInteractor.Load()) SetOpen(false);
        }

        private void OpenMainMenu()
        {
            SetOpen(false);
            _openMainMenu();
        }

        private void SetOpen(bool value)
        {
            _model.SetOpen(value);
            _view.Show(value);
            Time.timeScale = value ? 0f : 1f;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = value;
        }

        public void Dispose()
        {
            _view.PauseRequested -= Toggle;
            _view.MainMenuClicked -= OpenMainMenu;
            _view.SaveClicked -= Save;
            _view.LoadClicked -= Load;
            Time.timeScale = 1f;
        }
    }
}