using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(menuName = "Game/Character settings")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Header("Presentation")]
        public GameObject visualPrefab;
        public RuntimeAnimatorController controller;
        
        [Header("Stats")]
        public float health = 100;
        public float physicalDamage = 20;
        public float magicalDamage = 25;
        public float walkSpeed = 3.5f;
        public float runSpeed = 6;
        
        [Header("Attacks: seconds from animation start")]
        public float meleeDuration = 0.9f;
        public float meleeImpact = 0.35f;
        public float magicDuration = 1;
        public float magicImpact = 0.4f;
        public float meleeCooldown = 1.1f;
        public float magicCooldown = 2.2f;
        public float meleeReach = 2;
        public float meleeArc = 110;
        public float hitDuration = 0.35f;
        public float deathDuration = 2.5f;
        
        [Header("Magic")]
        public float maxMana = 100f;
        public float magicCost = 20f;
        public float manaRegenPerSec = 5f;
        public float projectileSpeed = 12;
        public float projectileLifetime = 3;
        public Color projectileColor = new Color(0.2f, 0.75f, 1);
        
        [Header("Enemy AI")]
        public float detectionDistance = 10f;
        public float loseTargetDistance = 14f;
        public float fleeHealthRatio = 0.25f;
        public float fleeDistance = 6f;
        public float rangedMinDistance = 4f;
        public float rangedMaxDistance = 9f;

        [Header("Boss AI")]
        public float bossStrongAttackRange = 8f;
        public float bossStrongAttackCooldown = 6f;
        public float bossRepositionDuration = 0.75f;
        public float enragedHealthRatio = 0.5f;
        public float enragedAttackSpeed = 1.5f;
    }
}
