using Actors;
using Actors.Stats;
using UnityEngine;

namespace Combat.Projectiles
{
    public sealed class ProjectileFactory
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private readonly Material _material;

        public ProjectileFactory(Material material)
        {
            _material = material;
        }
        
        public void Spawn(Combatant owner, CharacterDefinition definition, Vector3 direction)
        {
            Spawn(owner, definition, owner.AimPoint, direction, definition.magicalDamage,
                definition.projectileSpeed, definition.projectileColor, null);
        }
        
        public void Spawn(Combatant owner, CharacterDefinition definition, Vector3 position, Vector3 direction,
            float damageAmount, float speed, Color color, GameObject projectileEffect)
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Projectile";

            Collider projectileCollider = projectile.GetComponent<Collider>();
            projectileCollider.enabled = false;
            Object.Destroy(projectileCollider);

            projectile.transform.position = position;
            projectile.transform.localScale = Vector3.one * 0.28f;

            Renderer projectileRenderer = projectile.GetComponent<Renderer>();
            projectileRenderer.sharedMaterial = _material;

            MaterialPropertyBlock properties = new();
            properties.SetColor(BaseColor, color);
            properties.SetColor(ColorProperty, color);
            projectileRenderer.SetPropertyBlock(properties);

            if (projectileEffect)
                Object.Instantiate(projectileEffect, projectile.transform);

            Damage damage = new(damageAmount, DamageType.Magical, owner.Faction);

            projectile.AddComponent<MagicProjectile>().Initialize(
                direction * speed,
                definition.projectileLifetime,
                damage);
        }
    }
}
