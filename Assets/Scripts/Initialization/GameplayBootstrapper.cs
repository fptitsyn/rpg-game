using Actors;
using Actors.Enemies;
using Actors.Player;
using CameraScripts;
using Combat.Projectiles;
using SaveSystem;
using UI.InGame;
using UI.Menu;
using UI.Menu.Pause;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public sealed class GameplayBootstrapper : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] PlayerSetup player;

        [Header("Enemies")]
        [SerializeField] CharacterDefinition meleeEnemy;
        [SerializeField] CharacterDefinition rangedEnemy;

        [Header("Scene")]
        [SerializeField] Material projectileMaterial;
        [SerializeField] Camera gameCamera;
        [SerializeField] OrbitCamera orbitCamera;

        [Header("Input")]
        [SerializeField] InputActionAsset inputActions;

        [Header("UI")]
        [SerializeField] GameHud gameHud;
        [SerializeField] HealthBar playerHealthBar;
        
        [SerializeField] string mainMenuScene = "MainMenu";
        [SerializeField] PauseMenu pauseMenu;

        private CursorLockMode _previousCursorLock;
        private bool _previousCursorVisibility;

        private PauseMenuController _pauseController;

        private void Awake()
        {
            _previousCursorLock = Cursor.lockState;
            _previousCursorVisibility = Cursor.visible;
            
            InputActionMap playerActions = inputActions.FindActionMap("Player", throwIfNotFound: true);
            ProjectileFactory projectileFactory = new ProjectileFactory(projectileMaterial);

            player.Initialize(playerActions, projectileFactory, orbitCamera);

            IGameSaveRepository saveRepository = GameBootstrapper.Instance.Services.SaveRepository;
            GameSaveInteractor saveInteractor = new GameSaveInteractor(saveRepository, player);
            _pauseController = new PauseMenuController(new PauseMenuModel(), pauseMenu, saveInteractor, OpenMainMenu);
            
            playerHealthBar.Bind(player.Combatant.Health, gameCamera);
            gameHud.Bind(player.Combatant.Health, player.Combat);

            ActorFactory actorFactory = new ActorFactory(projectileFactory, gameCamera);
            EnemySpawner enemySpawner = new EnemySpawner(actorFactory);
            enemySpawner.Spawn(meleeEnemy, player.Combatant, false, Random.Range(2, 4));
            enemySpawner.Spawn(rangedEnemy, player.Combatant, true, Random.Range(2, 4));

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
    }
}