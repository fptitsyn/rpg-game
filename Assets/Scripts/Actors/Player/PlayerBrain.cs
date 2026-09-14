using CameraScripts;
using Game;
using UnityEngine;

namespace Actors.Player
{
    public sealed class PlayerBrain : MonoBehaviour
    {
        private IPlayerInput _input;
        private PlayerMotor _motor;
        private ActorCombat _combat;
        private Combatant _actor;
        private CharacterDefinition _definition;
        private OrbitCamera _orbit;

        public void Initialize(IPlayerInput controls, PlayerMotor movement, ActorCombat attacks,
            Combatant owner, CharacterDefinition settings, OrbitCamera cameraRig)
        {
            _input = controls;
            _motor = movement;
            _combat = attacks;
            _actor = owner;
            _definition = settings;
            _orbit = cameraRig;
        }
        
        private void Update()
        {
            _motor.SetVelocity(Vector3.zero);
            if (!_actor.IsAlive) return;
            if (_input.ReleaseCursorPressed)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (_input.MeleePressed)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                return;
            }
            
            _orbit.Rotate(_input.Look);
            if (_combat.IsLocked) return;
            
            Vector3 direction = Quaternion.Euler(0, _orbit.Yaw, 0) * new Vector3(_input.Move.x, 0, _input.Move.y);
            if (_input.MeleePressed || _input.MagicPressed)
            {
                transform.rotation = Quaternion.Euler(0, _orbit.Yaw, 0);
                _combat.TryAttack(_input.MagicPressed);
            }
            else if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    Quaternion.LookRotation(direction), 720 * Time.deltaTime);
                _motor.SetVelocity(direction * (_input.Sprint ? _definition.runSpeed : _definition.walkSpeed));
            }
        }
    }
}
