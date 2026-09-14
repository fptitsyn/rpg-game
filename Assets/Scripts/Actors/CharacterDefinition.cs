using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(menuName = "Game/Character settings")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Header("Presentation")]
        public GameObject visualPrefab;
        public RuntimeAnimatorController controller;
        public bool legacyMonsterAnimator;
        [Min(0.1f)] public float visualScale = 1;
        public float visualYaw;
        [Header("Stats")]
        [Min(1)] public float health = 100;
        [Min(0)] public float physicalDamage = 20;
        [Min(0)] public float magicalDamage = 25;
        [Min(0.1f)] public float walkSpeed = 3.5f;
        [Min(0.1f)] public float runSpeed = 6;
        [Header("Attacks: seconds from animation start")]
        [Min(0.01f)] public float meleeDuration = 0.9f;
        [Min(0)] public float meleeImpact = 0.35f;
        [Min(0.01f)] public float magicDuration = 1;
        [Min(0)] public float magicImpact = 0.4f;
        [Min(0.01f)] public float meleeCooldown = 1.1f;
        [Min(0.01f)] public float magicCooldown = 2.2f;
        [Min(0.1f)] public float meleeReach = 2;
        [Range(10, 180)] public float meleeArc = 110;
        [Min(0.01f)] public float hitDuration = 0.35f;
        [Min(0.1f)] public float deathDuration = 2.5f;
        [Header("Magic")]
        [Min(0.1f)] public float projectileSpeed = 12;
        [Min(0.1f)] public float projectileLifetime = 3;
        public Color projectileColor = new Color(0.2f, 0.75f, 1);

        private void OnValidate()
        {
            meleeImpact = Mathf.Clamp(meleeImpact, 0, meleeDuration);
            magicImpact = Mathf.Clamp(magicImpact, 0, magicDuration);
        }
    }
}
