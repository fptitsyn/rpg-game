using System.Collections.Generic;
using UnityEngine;

namespace Actors.Animations
{
    public interface ICharacterAnimation
    {
        void SetSpeed(float speed);
        void PlayAction(string state, float duration);
        void ResumeLocomotion();
    }

    public sealed class CharacterAnimation : MonoBehaviour, ICharacterAnimation
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int ActionSpeedHash = Animator.StringToHash("ActionSpeed");

        private readonly Dictionary<string, AnimationClip> _clips = new();
        private readonly List<KeyValuePair<AnimationClip, AnimationClip>> _overrides = new();

        private Animator _animator;
        private CharacterDefinition _definition;
        private bool _action;

        public void Initialize(Animator animator, CharacterDefinition definition)
        {
            _animator = animator;
            _definition = definition;

            _animator.applyRootMotion = false;
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            CacheClips(definition.controller);
        }

        public void SetSpeed(float speed)
        {
            float normalizedSpeed = Mathf.Clamp01(speed / _definition.runSpeed);
            _animator.SetFloat(SpeedHash, _action ? 0f : normalizedSpeed, 0.1f, Time.deltaTime);
        }

        public void PlayAction(string state, float duration)
        {
            _action = true;
            _animator.SetFloat(SpeedHash, 0f);

            float animationSpeed = _clips[state].length / Mathf.Max(0.01f, duration);

            _animator.SetFloat(ActionSpeedHash, animationSpeed);
            _animator.CrossFadeInFixedTime("Base Layer." + state, 0.06f, 0, 0f);
        }

        public void ResumeLocomotion()
        {
            if (!_action)
                return;

            _action = false;
            _animator.SetFloat(ActionSpeedHash, 1f);
            _animator.CrossFadeInFixedTime("Base Layer.Locomotion", 0.1f, 0, 0f);
        }

        private void CacheClips(RuntimeAnimatorController controller)
        {
            _clips.Clear();

            if (controller is AnimatorOverrideController overrideController)
            {
                _overrides.Clear();
                overrideController.GetOverrides(_overrides);

                foreach (KeyValuePair<AnimationClip, AnimationClip> pair in _overrides)
                    _clips[pair.Key.name] = pair.Value;

                return;
            }

            foreach (AnimationClip clip in controller.animationClips)
                _clips[clip.name] = clip;
        }   
    }
}