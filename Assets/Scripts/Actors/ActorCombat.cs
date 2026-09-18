using Actors.Animations;
using Actors.Stats;
using Combat;
using UnityEngine;

namespace Actors
{
    public sealed class ActorCombat : MonoBehaviour
    {
        private Combatant _actor;
        private CharacterDefinition _definition;
        private ICharacterAnimation _animationView;
        private IAttackEffect _melee;
        private IAttackEffect _magic;
        private readonly AttackTimeline _timeline = new AttackTimeline();
        private float _hitUntil;
        private float _nextMelee;
        private float _nextMagic;
        private bool _wasLocked;
        
        public bool IsLocked => !_actor.IsAlive || Time.time < _hitUntil || _timeline.IsRunning;
        public float MagicRemaining => Mathf.Max(0, _nextMagic - Time.time);
        public float MagicCooldown => _definition.magicCooldown;

        public void Initialize(Combatant owner, CharacterDefinition settings, ICharacterAnimation view,
            IAttackEffect physical, IAttackEffect magical)
        {
            _actor = owner;
            _definition = settings;
            _animationView = view;
            _melee = physical;
            _magic = magical;
            _actor.Health.Damaged += OnDamaged;
            _actor.Health.Died += OnDied;
        }
        
        public bool TryAttack(bool magical)
        {
            if (IsLocked || Time.time < (magical ? _nextMagic : _nextMelee)) return false;

            if (magical && _actor.Mana != null && _actor.Mana.Current < _definition.magicCost) return false;

            float duration = magical ? _definition.magicDuration : _definition.meleeDuration;
            float impact = magical ? _definition.magicImpact : _definition.meleeImpact;

            if (!_timeline.TryStart(impact, duration, magical ? _magic : _melee)) return false;

            if (magical && _actor.Mana != null) _actor.Mana.TrySpend(_definition.magicCost);

            if (magical) _nextMagic = Time.time + _definition.magicCooldown;
            else _nextMelee = Time.time + _definition.meleeCooldown;

            _animationView.PlayAction(magical ? "Magic" : "Melee");
            _wasLocked = true;
            return true;
        }
        
        private void Update()
        {
            if (!_actor || !_actor.IsAlive) return;
            
            _timeline.Tick(Time.deltaTime);
            if (_wasLocked && !IsLocked)
            {
                _animationView.ResumeLocomotion();
            }
            _wasLocked = IsLocked;
        }
        
        private void OnDamaged(Damage damage)
        {
            _timeline.Cancel();
            _hitUntil = Time.time + _definition.hitDuration;
            _wasLocked = true;
            _animationView.PlayAction("Hit");
        }
        
        private void OnDied()
        {
            _timeline.Cancel();
            _animationView.PlayAction("Death");
            if (_actor.Faction == Faction.Enemy) Destroy(gameObject, _definition.deathDuration);
        }
        
        private void OnDisable() => _timeline.Cancel();
        
        private void OnDestroy()
        {
            if (_actor == null || _actor.Health == null) return;
            _actor.Health.Damaged -= OnDamaged;
            _actor.Health.Died -= OnDied;
        }
        
        public void ResetAfterLoad()
        {
            _timeline.Cancel();
            _hitUntil = 0f;
            _nextMelee = 0f;
            _nextMagic = 0f;
            _wasLocked = false;
            _animationView.ResumeLocomotion();
        }
    }
}
