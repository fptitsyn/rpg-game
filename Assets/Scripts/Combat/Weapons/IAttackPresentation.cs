using UnityEngine;

namespace Combat.Weapons
{
    public interface IAttackPresentation
    {
        Vector3 AttackPoint { get; }
        Color Color { get; }
        GameObject ProjectileEffect { get; }

        void Play();
    }
}