using Actors;
using Actors.Enemies;
using Actors.Player;
using Actors.Spawning;
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
        [SerializeField] Transform enemySpawnPointsRoot;
        [SerializeField] EnemyMode enemyMode;

        [Header("Boss")]
        [SerializeField] private CharacterDefinition bossDefinition;
        [SerializeField] BossSpawnPoint bossSpawnPoint;
        
        [Header("Scene")]
        [SerializeField] private Material projectileMaterial;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private OrbitCamera orbitCamera;

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("UI")]
        [SerializeField] private GameHud gameHud;
        [SerializeField] private HealthBar playerHealthBar;
        [SerializeField] private ManaBar playerManaBar;
        
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private PauseMenu pauseMenu;

        private CursorLockMode _previousCursorLock;
        private bool _previousCursorVisibility;

        private ActorFactory _actorFactory;
        private Combatant _player;
        private EnemyWave _enemyWave;

        private PauseMenuController _pauseController;

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
            playerManaBar.Bind(playerSetup.Combatant.Mana);
            gameHud.Bind(playerSetup.Combatant.Health, playerSetup.Combat);

            _actorFactory = new ActorFactory(projectileFactory, gameCamera);

            _enemyWave = new EnemyWave();
            _enemyWave.AllEnemiesDead += SpawnBoss;
            MeleeEnemySpawnPoint[] meleeSpawnPoints =
                enemySpawnPointsRoot.GetComponentsInChildren<MeleeEnemySpawnPoint>();

            foreach (MeleeEnemySpawnPoint spawnPoint in meleeSpawnPoints)
            {
                Combatant enemy = spawnPoint.Spawn(_actorFactory, _player, enemyMode);
                _enemyWave.Add(enemy);
            }

            RangedEnemySpawnPoint[] rangedSpawnPoints =
                enemySpawnPointsRoot.GetComponentsInChildren<RangedEnemySpawnPoint>();

            foreach (RangedEnemySpawnPoint spawnPoint in rangedSpawnPoints)
            {
                Combatant enemy = spawnPoint.Spawn(_actorFactory, _player, enemyMode);
                _enemyWave.Add(enemy);
            }

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
            _enemyWave.AllEnemiesDead -= SpawnBoss;
            
            Cursor.lockState = _previousCursorLock;
            Cursor.visible = _previousCursorVisibility;
        }

        private void SpawnBoss()
        {
            bossSpawnPoint.Spawn(_actorFactory, _player, enemyMode);
        }
    }
}