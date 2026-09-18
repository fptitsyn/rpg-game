using UnityEngine;

namespace Actors.Animations
{
    public interface ICharacterAnimation
    {
        void SetSpeed(float speed);
        void PlayAction(string state);
        void ResumeLocomotion();
    }

    public sealed class CharacterAnimation : MonoBehaviour, ICharacterAnimation
    {
        private Animator _animator;
        private bool _legacy;
        private CharacterDefinition _definition;
        private bool _action;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int ActionSpeed = Animator.StringToHash("ActionSpeed");

        public void Initialize(Animator target, CharacterDefinition settings)
        {
            _animator = target;
            _definition = settings;
            _legacy = settings.monsterAnimator;
            _animator.applyRootMotion = false;
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
        
        public void SetSpeed(float speed)
        {
            if (_legacy)
            {
                if (Has("Walking", AnimatorControllerParameterType.Bool))
                    _animator.SetBool(Walking, !_action && speed > 0.1f);
            }
            else _animator.SetFloat(Speed, _action ? 0 : speed, 0.1f, Time.deltaTime);
        }
        
        public void PlayAction(string state)
        {
            _action = true;
            if (!_legacy)
            {
                float duration = state switch
                {
                    "Hit" => _definition.hitDuration,
                    "Magic" => _definition.magicDuration,
                    _ => _definition.meleeDuration
                };
                float rate = 1;
                if (state != "Death")
                    foreach (var clip in _animator.runtimeAnimatorController.animationClips)
                    {
                        if (clip.name == state)
                        {
                            rate = clip.length / Mathf.Max(0.01f, duration);
                            break;
                        }
                    }
                
                _animator.SetFloat(ActionSpeed, rate);
                _animator.CrossFadeInFixedTime("Base Layer." + state, 0.06f, 0, 0);
                return;
            }
            
            string parameter = state switch
            {
                "Hit" => "TakeDamage",
                "Death" => "Death",
                _ => "Attack"
            };
            
            if (Has("Walking", AnimatorControllerParameterType.Bool)) _animator.SetBool(Walking, false);
            foreach (var p in _animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger) _animator.ResetTrigger(p.nameHash);
            }
            if (Has(parameter, AnimatorControllerParameterType.Trigger)) _animator.SetTrigger(parameter);
        }
        
        public void ResumeLocomotion()
        {
            if (!_action) return;
            _action = false;
            if (!_legacy) _animator.CrossFadeInFixedTime("Base Layer.Locomotion", 0.1f, 0, 0);
        }
        
        private bool Has(string nameToCheck, AnimatorControllerParameterType type)
        {
            foreach (var p in _animator.parameters) if (p.name == nameToCheck && p.type == type) return true;
            return false;
        }
    }
}
