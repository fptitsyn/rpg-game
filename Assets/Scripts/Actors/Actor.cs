using UnityEngine;

namespace Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] protected float maxHealth;
        public float CurrentHealth { get; protected set; }

        protected virtual void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public virtual void ReceiveDamage(float damage)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, maxHealth);
        }
    }
}