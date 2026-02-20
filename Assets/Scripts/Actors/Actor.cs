using UnityEngine;

namespace Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField] protected float maxHealth;
        protected float CurrentHealth;
        
        protected virtual void ReceiveDamage(float damage)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, 100f);
        }
    }
}