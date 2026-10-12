using UnityEngine;

namespace Combat.Weapons
{
    [CreateAssetMenu(menuName = "Game/Combat/Weapon")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        public WeaponType type;

        [Header("Visual")]
        public GameObject visualPrefab;
        public WeaponEffectDefinition defaultEffect = new();

        [Header("Stats")]
        public float damageMultiplier = 1f;
        public float strongDamageMultiplier = 1.75f;
        public float rangeMultiplier = 1f;
        public float projectileSpeedMultiplier = 1f;
    }
}