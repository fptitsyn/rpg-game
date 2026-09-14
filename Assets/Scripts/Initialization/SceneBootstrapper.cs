using Actors;
using Actors.Enemies;
using Actors.Player;
using CameraScripts;
using Combat.Projectiles;
using Game;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Initialization
{
    public sealed class SceneBootstrapper : MonoBehaviour
    {
        public CharacterDefinition player;
        public CharacterDefinition meleeEnemy;
        public CharacterDefinition rangedEnemy;
        public Material projectileMaterial;
        public NavMeshSurface navigation;
        public Camera gameCamera;
        public Vector3 playerSpawn = new Vector3(0, 0, -12);
        private PlayerInput _input;
        private UiFactory _ui;
        private CursorLockMode _previousLock;
        private bool _previousVisible;

        private void Awake()
        {
            _previousLock = Cursor.lockState;
            _previousVisible = Cursor.visible;
            
            if (!Valid(player) || !Valid(meleeEnemy) || !Valid(rangedEnemy) ||
                navigation == null || gameCamera == null || projectileMaterial == null)
            {
                Debug.LogError("Scene setup is incomplete. Assign character settings, camera, navigation and projectile material on SceneBootstrapper.");
                enabled = false;
                return;
            }
            
            // Bake before adding actors; only World layer participates in navigation.
            navigation.BuildNavMesh();
            if (!NavMesh.SamplePosition(playerSpawn, out var start, 2, NavMesh.AllAreas))
            {
                Debug.LogError("Game: player spawn is outside NavMesh.");
                enabled = false;
                return;
            }
            
            _input = new PlayerInput();
            _ui = new UiFactory();
            
            var factory = new ActorFactory(new ProjectileFactory(projectileMaterial), _ui, gameCamera);
            var orbit = gameCamera.gameObject.AddComponent<OrbitCamera>();
            var hero = factory.CreatePlayer(player, start.position, _input, orbit);
            
            var spawner = new EnemySpawner(factory);
            spawner.Spawn(meleeEnemy, hero, false, Random.Range(2, 4));
            spawner.Spawn(rangedEnemy, hero, true, Random.Range(2, 4));
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private static bool Valid(CharacterDefinition settings) => settings != null &&
            settings.visualPrefab != null && settings.controller != null &&
            settings.visualPrefab.GetComponentInChildren<Animator>() != null;
        
        private void OnDestroy()
        {
            _input?.Dispose();
            _ui?.Dispose();
            Cursor.lockState = _previousLock; Cursor.visible = _previousVisible;
        }
    }
}
