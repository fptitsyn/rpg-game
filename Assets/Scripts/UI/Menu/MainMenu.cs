using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject mainPanel;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] string gameScene = "Game";

        private void Awake()
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        public void Play()
        {
            SceneManager.LoadScene(gameScene);
        }

        public void ToggleSettings()
        {
            settingsPanel.SetActive(mainPanel.activeInHierarchy);
            mainPanel.SetActive(!mainPanel.activeInHierarchy);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}