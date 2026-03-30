using System.Collections.Generic;
using UnityEngine;

namespace Actors.Enemies.BehaviourTree.Base
{
    public class Timer : Node
    {
        private float _delay;
        private float _time;

        private Animator _animator;
        private GameObject _hammer;

        private AudioSource _audioSource;

        public delegate void TickEnded();
        public event TickEnded OnTickEnded;

        public Timer(float delay, TickEnded onTickEnded = null)
        {
            _delay = delay;
            _time = _delay;
            OnTickEnded = onTickEnded;
        }
        public Timer(float delay, List<Node> children, TickEnded onTickEnded = null)
            : base(children)
        {
            _delay = delay;
            _time = _delay;
            OnTickEnded = onTickEnded;
        }
        
        public override NodeState Evaluate()
        {
            if (!HasChildren) return NodeState.Failure;

            if (_time <= 0)
            {
                _time = _delay;
                _state = children[0].Evaluate();
                OnTickEnded?.Invoke();
                _state = NodeState.Success;
            }
            else
            {
                _time -= Time.deltaTime;
                _state = NodeState.Running;
            }
            
            return _state;
        }
    }
}