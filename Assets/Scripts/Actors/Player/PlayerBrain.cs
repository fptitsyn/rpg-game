using System;
using CameraScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public sealed class PlayerBrain : MonoBehaviour
    {
        private InputActionMap _inputMap;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private InputAction _attackAction;
        private InputAction _magicAction;
        private InputAction _releaseCursorAction;

        private PlayerMotor _motor;
        private ActorCombat _combat;
        private Combatant _actor;
        private CharacterDefinition _definition;
        private OrbitCamera _orbit;

        public void Initialize(InputActionMap controls, PlayerMotor movement, ActorCombat attacks,
            Combatant owner, CharacterDefinition settings, OrbitCamera cameraRig)
        {
            _inputMap = controls ?? throw new ArgumentNullException(nameof(controls));

            _motor = movement;
            _combat = attacks;
            _actor = owner;
            _definition = settings;
            _orbit = cameraRig;

            // set in inspector?
            _moveAction = FindAction("Move");
            _lookAction = FindAction("Look");
            _sprintAction = FindAction("Sprint");
            _attackAction = FindAction("Attack");
            _magicAction = FindAction("Magic Attack");

            _inputMap.Enable();
        }

        private void Update()
        {
            _motor.SetVelocity(Vector3.zero);

            if (Time.timeScale == 0f)
                return;
            
            if (!_actor.IsAlive)
                return;

            bool attackPressed = _attackAction.WasPressedThisFrame();
            bool magicPressed = _magicAction.WasPressedThisFrame();

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (attackPressed)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                return;
            }

            Vector2 look = _lookAction.ReadValue<Vector2>();
            _orbit.Rotate(look);

            if (_combat.IsLocked)
            {
                return;
            }

            Vector2 move = Vector2.ClampMagnitude(_moveAction.ReadValue<Vector2>(), 1);

            Vector3 direction = Quaternion.Euler(0, _orbit.Yaw, 0) * new Vector3(move.x, 0, move.y);

            if (attackPressed || magicPressed)
            {
                transform.rotation = Quaternion.Euler(0, _orbit.Yaw, 0);

                _combat.TryAttack(magicPressed);
            }
            else if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 720 * Time.deltaTime);
                float speed = _sprintAction.IsPressed() ? _definition.runSpeed : _definition.walkSpeed;
                
                _motor.SetVelocity(direction * speed);
            }
        }

        private InputAction FindAction(string actionName)
        {
            return _inputMap.FindAction(actionName, throwIfNotFound: true);
        }

        private void OnDestroy()
        {
            _inputMap?.Disable();
        }
    }
}