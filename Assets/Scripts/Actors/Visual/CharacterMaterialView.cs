using UnityEngine;

namespace Actors.Visual
{
    public sealed class CharacterMaterialView : MonoBehaviour
    {
        [SerializeField] Renderer[] targetRenderers;

        public void Apply(Material material)
        {
            foreach (Renderer targetRenderer in targetRenderers)
                targetRenderer.sharedMaterial = material;
        }
    }
}