using System;
using Actors;
using Actors.Health;
using Game;
using UnityEngine;

namespace Combat.Projectiles
{
    public sealed class MagicProjectile : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _remaining;
        private Damage _damage;
        private const float Radius = 0.14f;

        public void Initialize(Vector3 speed, float lifetime, Damage hit)
        {
            _velocity = speed;
            _remaining = lifetime;
            _damage = hit;
        }

        private void Update()
        {
            float step = Mathf.Min(Time.deltaTime, _remaining);
            if (step <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            Vector3 origin = transform.position;
            // SphereCast does not report an initial overlap; handle it separately.
            foreach (var coll in Physics.OverlapSphere(origin, Radius,
                CombatPhysics.CombatMask, QueryTriggerInteraction.Ignore))
            {
                if (Impact(coll)) return;
            }
            
            Vector3 displacement = _velocity * step;
            var hits = Physics.SphereCastAll(origin, Radius, displacement.normalized,
                displacement.magnitude, CombatPhysics.CombatMask, QueryTriggerInteraction.Ignore);
            
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                if (Impact(hit.collider))
                {
                    return;
                }
            }
            
            transform.position += displacement;
            
            _remaining -= step;
            if (_remaining <= 0)
            {
                Destroy(gameObject);
            }
        }
        
        private bool Impact(Collider coll)
        {
            Combatant target = coll.GetComponentInParent<Combatant>();
            if (target)
            {
                if (!target.IsAlive || target.Faction == _damage.Source)
                {
                    return false;
                }
                
                IDamageReceiver receiver = target;
                receiver.Receive(_damage);
            }
            Destroy(gameObject);
            enabled = false;
            return true;
        }
    }
}
