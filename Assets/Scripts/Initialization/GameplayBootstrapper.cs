using Actors;
using Actors.Enemies;
using Actors.Player;
using CameraScripts;
using Combat.Projectiles;
using SaveSystem;
using UI.InGame;
using UI.Menu.Pause;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public sealed class GameplayBootstrapper : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerSetup playerSetup;

        [Header("Enemies")]
        [SerializeField] private CharacterDefinition meleeEnemy;
        [SerializeField] private CharacterDefinition rangedEnemy;
        [SerializeField] private EnemyMode enemyMode;

        [Header("Boss")]
        [SerializeField] private CharacterDefinition bossDefinition;
        [SerializeField] private Transform bossSpawnPoint;
        
        [Header("Scene")]
        [SerializeField] private Material projectileMaterial;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private OrbitCamera orbitCamera;

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("UI")]
        [SerializeField] private GameHud gameHud;
        [SerializeField] private HealthBar playerHealthBar;
        
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private PauseMenu pauseMenu;

        private CursorLockMode _previousCursorLock;
        private bool _previousCursorVisibility;

        private PauseMenuController _pauseController;

        private EnemySpawner _enemySpawner;
        private EnemyWave _enemyWave;
        private Combatant _player;

        private void Awake()
        {
            _previousCursorLock = Cursor.lockState;
            _previousCursorVisibility = Cursor.visible;
            
            InputActionMap playerActions = inputActions.FindActionMap("Player", throwIfNotFound: true);
            ProjectileFactory projectileFactory = new ProjectileFactory(projectileMaterial);

            playerSetup.Initialize(playerActions, projectileFactory, orbitCamera);

            _player = playerSetup.Combatant;

            IGameSaveRepository saveRepository = GameBootstrapper.Instance.Services.SaveRepository;
            GameSaveInteractor saveInteractor = new GameSaveInteractor(saveRepository, playerSetup);
            _pauseController = new PauseMenuController(new PauseMenuModel(), pauseMenu, saveInteractor, OpenMainMenu);
            
            playerHealthBar.Bind(playerSetup.Combatant.Health, gameCamera);
            gameHud.Bind(playerSetup.Combatant.Health, playerSetup.Combat);

            ActorFactory actorFactory = new ActorFactory(projectileFactory, gameCamera);
            _enemySpawner = new EnemySpawner(actorFactory, enemyMode);

            _enemyWave = new EnemyWave();
            _enemyWave.AllEnemiesDead += SpawnBoss;
            
            _enemyWave.Add(_enemySpawner.Spawn(meleeEnemy, _player, false, Random.Range(1, 3)));
            _enemyWave.Add(_enemySpawner.Spawn(rangedEnemy, _player, true, Random.Range(1, 3)));
            _enemyWave.Begin();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            pauseMenu.MainMenuClicked += OpenMainMenu;
        }

        private void OpenMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }
        
        private void OnDestroy()
        {
            pauseMenu.MainMenuClicked -= OpenMainMenu;
            
            Cursor.lockState = _previousCursorLock;
            Cursor.visible = _previousCursorVisibility;
        }

        private void SpawnBoss()
        {
            _enemySpawner.SpawnBoss(bossDefinition, bossSpawnPoint.position, _player, enemyMode);
        }
    }
}