using UnityEngine;

namespace Combat.Weapons
{
    public sealed class WeaponView : MonoBehaviour, IAttackPresentation
    {
        // private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        // private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        // private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        
        [SerializeField] Transform attackPoint;
        [SerializeField] AudioSource audioSource;
        // [SerializeField] Renderer[] weaponRenderers;

        private AudioClip _attackSound;
        private GameObject _attackEffect;
        private GameObject _projectileEffect;
        // private Color _color;
        // private MaterialPropertyBlock _properties;

        public Vector3 AttackPoint => attackPoint.position;
        // public Color Color => _color;
        public GameObject ProjectileEffect => _projectileEffect;

        public void Initialize(WeaponEffectDefinition effect)
        {
            _attackSound = effect.attackSound;
            _attackEffect = effect.attackEffect;
            _projectileEffect = effect.projectileEffect;
            // _color = effect.color;
            // _properties = new MaterialPropertyBlock();

            // foreach (Renderer weaponRenderer in weaponRenderers)
            // {
            //     weaponRenderer.GetPropertyBlock(_properties);
            //     _properties.SetColor(BaseColor, _color);
            //     _properties.SetColor(ColorProperty, _color);
            //     _properties.SetColor(EmissionColor, _color);
            //     weaponRenderer.SetPropertyBlock(_properties);
            // }
        }

        public void Play()
        {
            audioSource.PlayOneShot(_attackSound);
            // Instantiate(_attackEffect, attackPoint.position, attackPoint.rotation);
        }
    }
}