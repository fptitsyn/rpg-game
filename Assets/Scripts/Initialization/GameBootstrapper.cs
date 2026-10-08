using System.IO;
using SaveSystem;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] string mainMenuScene = "MainMenu";
        [SerializeField] AudioMixer audioMixer;

        private static GameBootstrapper _instance;

        public static GameBootstrapper Instance => _instance;
        public ServiceLocator Services { get; private set; }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(gameObject);

            IAudioService audioService = new AudioService(audioMixer);

            string savePath = Path.Combine(Application.persistentDataPath, $"save.json_{FileGameSaveRepository.currentSaveNumber}");
            IGameSaveRepository saveRepository = new FileGameSaveRepository(savePath);
            
            Services = new ServiceLocator(audioService, saveRepository);
        }

        private void Start()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}