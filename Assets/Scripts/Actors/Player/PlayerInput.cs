using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public interface IPlayerInput
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool Sprint { get; }
        bool MeleePressed { get; }
        bool MagicPressed { get; }
        bool ReleaseCursorPressed { get; }
    }

    public sealed class PlayerInput : IPlayerInput, IDisposable
    {
        private readonly InputActionMap map = new InputActionMap("Gameplay");
        private readonly InputAction move, look, sprint, melee, magic, release;
        
        public PlayerInput()
        {
            move = map.AddAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            look = map.AddAction("Look", InputActionType.Value, "<Mouse>/delta");
            sprint = map.AddAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");
            sprint.AddBinding("<Keyboard>/rightShift");
            melee = map.AddAction("Melee", InputActionType.Button, "<Mouse>/leftButton");
            magic = map.AddAction("Magic", InputActionType.Button, "<Mouse>/rightButton");
            release = map.AddAction("Release cursor", InputActionType.Button, "<Keyboard>/escape");
            map.Enable();
        }
        
        public Vector2 Move => Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1);
        public Vector2 Look => look.ReadValue<Vector2>();
        public bool Sprint => sprint.IsPressed();
        public bool MeleePressed => melee.WasPressedThisFrame();
        public bool MagicPressed => magic.WasPressedThisFrame();
        public bool ReleaseCursorPressed => release.WasPressedThisFrame();
        public void Dispose() => map.Dispose();
    }
}
