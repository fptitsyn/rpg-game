using UnityEngine;

namespace Combat.Weapons
{
    public enum ElementType
    {
        Ice,
        Fire,
        Earth,
        Aether
    }

    [CreateAssetMenu(menuName = "Game/Combat/Element")]
    public sealed class ElementDefinition : ScriptableObject
    {
        public ElementType type;
        public float damageMultiplier = 1f;

        [Header("Melee weapon")]
        public WeaponEffectDefinition meleeEffect = new();

        [Header("Ranged weapon")]
        public WeaponEffectDefinition rangedEffect = new();

        public WeaponEffectDefinition GetEffect(WeaponType weaponType)
        {
            return weaponType == WeaponType.Melee ? meleeEffect : rangedEffect;
        }
    }
}