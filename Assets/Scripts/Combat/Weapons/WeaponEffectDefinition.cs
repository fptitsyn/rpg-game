using System;
using UnityEngine;

namespace Combat.Weapons
{
    [Serializable]
    public class WeaponEffectDefinition
    {
        // public Color color = Color.white;
        public AudioClip attackSound;
        public GameObject attackEffect;
        public GameObject projectileEffect;
    }
}