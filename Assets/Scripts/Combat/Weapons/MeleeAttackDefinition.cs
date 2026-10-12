using Actors.Animations;
using UnityEngine;

namespace Combat.Weapons
{
    [CreateAssetMenu(menuName = "Game/Combat/Melee attack")]
    public sealed class MeleeAttackDefinition : ScriptableObject
    {
        public MeleeAttackSource source;

        [Header("Hand attack")]
        public float handDamageMultiplier = 1f;
        public float handRangeMultiplier = 1f;

        [Header("Weapon attack")]
        public WeaponDefinition weapon;

        public AttackAnimation AnimationVariant =>
            source == MeleeAttackSource.Hand ? AttackAnimation.Attack1 : AttackAnimation.Attack2;

        public float DamageMultiplier =>
            source == MeleeAttackSource.Hand ? handDamageMultiplier : weapon.damageMultiplier;

        public float RangeMultiplier =>
            source == MeleeAttackSource.Hand ? handRangeMultiplier : weapon.rangeMultiplier;
    }
}