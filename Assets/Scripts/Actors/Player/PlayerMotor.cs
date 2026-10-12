using Actors.Animations;
using UnityEngine;

namespace Actors.Player
{
    public sealed class PlayerMotor : MonoBehaviour
    {
        private CharacterController _controller;
        private ActorCombat _combat;
        private ICharacterAnimation _animationView;
        private Vector3 _desiredVelocity;
        private float _verticalSpeed;

        public void Initialize(CharacterController body, ActorCombat attacks, ICharacterAnimation view)
        {
            _controller = body;
            _combat = attacks;
            _animationView = view;
        }
        
        public void SetVelocity(Vector3 velocity) => _desiredVelocity = velocity;
        
        private void LateUpdate()
        {
            if (!_controller || !_controller.enabled) return;
            
            if (_controller.isGrounded && _verticalSpeed < 0)
            {
                _verticalSpeed = -2;
            }
            _verticalSpeed += Physics.gravity.y * Time.deltaTime;
            
            Vector3 horizontal = _combat.IsLocked ? Vector3.zero : _desiredVelocity;
            _controller.Move((horizontal + Vector3.up * _verticalSpeed) * Time.deltaTime);
            
            Vector3 actual = _controller.velocity;
            actual.y = 0;
            _animationView.SetSpeed(actual.magnitude);
        }
        
        public void ResetMovement()
        {
            _desiredVelocity = Vector3.zero;
            _verticalSpeed = 0f;
        }
    }
}
