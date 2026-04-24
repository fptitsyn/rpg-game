using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public class Player : Actor
    {
        [Header("Components")]
        [SerializeField] private GameObject freeLookCamera;
        [SerializeField] private float sensitivity = 20f;

        private PlayerStateMachine stateMachine;
        private InputAction lookAction;
        private InputAction walkAction;
        private InputAction sprintAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction magicAction;

        public static Action PlayerDied; 
        
        private void Start()
        {
            stateMachine = GetComponent<PlayerStateMachine>();

            lookAction = InputSystem.actions.FindAction("Look");
            walkAction = InputSystem.actions.FindAction("Move");
            sprintAction = InputSystem.actions.FindAction("Sprint");
            jumpAction = InputSystem.actions.FindAction("Jump");
            attackAction = InputSystem.actions.FindAction("Attack");
            magicAction = InputSystem.actions.FindAction("Magic Attack");

            var inputAxisController = freeLookCamera.GetComponent<CinemachineInputAxisController>();
            foreach (var c in inputAxisController.Controllers)
                c.Input.Gain = sensitivity;
        }

        private void Update()
        {
            stateMachine.MoveInput = walkAction.ReadValue<Vector2>();
            stateMachine.SprintHeld = sprintAction.IsPressed();
            stateMachine.JumpPressed = jumpAction.IsPressed();
            stateMachine.AttackPressed = attackAction.IsPressed();
            stateMachine.MagicPressed = magicAction.IsPressed();

            Vector2 lookValue = lookAction.ReadValue<Vector2>() * (sensitivity * Time.deltaTime);
            transform.Rotate(Vector3.up, lookValue.x);
        }

        public override void ReceiveDamage(float damage)
        {
            base.ReceiveDamage(damage);
            Debug.Log(CurrentHealth);
            if (CurrentHealth <= 0)
            {
                stateMachine.Die();
                PlayerDied?.Invoke();
            }
            else
            {
                stateMachine.TakeDamage();
            }
        }
    }
}