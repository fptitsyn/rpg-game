using Actors.Animations;
using UnityEngine;

namespace Combat.Weapons
{
    [CreateAssetMenu(menuName = "Game/Combat/Ranged attack")]
    public sealed class RangedAttackDefinition : ScriptableObject
    {
        [Header("Visual")]
        public Material characterMaterial;
        public Color projectileColor = Color.white;

        [Header("Animation")]
        public AttackAnimation animationVariant = AttackAnimation.Attack1;
        public float preparationDuration = 0.8f;

        [Header("Stats")]
        public float damageMultiplier = 1f;
        public float projectileSpeedMultiplier = 1f;
    }
}