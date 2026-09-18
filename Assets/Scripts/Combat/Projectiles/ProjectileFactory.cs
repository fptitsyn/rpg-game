using Actors;
using Actors.Stats;
using UnityEngine;

namespace Combat.Projectiles
{
    public sealed class ProjectileFactory
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private readonly Material _material;

        public ProjectileFactory(Material material)
        {
            _material = material;
        }
        
        public void Spawn(Combatant owner, CharacterDefinition settings, Vector3 direction)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Magic projectile";
            
            var collider = go.GetComponent<Collider>();
            collider.enabled = false;
            Object.Destroy(collider);
            
            go.transform.position = owner.AimPoint;
            go.transform.localScale = Vector3.one * 0.28f;
            
            var renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = _material;
            
            var properties = new MaterialPropertyBlock();
            properties.SetColor(BaseColor, settings.projectileColor);
            renderer.SetPropertyBlock(properties);
            
            go.AddComponent<MagicProjectile>().Initialize(direction * settings.projectileSpeed,
                settings.projectileLifetime,
                new Damage(settings.magicalDamage, DamageType.Magical, owner.Faction));
        }
    }
}
