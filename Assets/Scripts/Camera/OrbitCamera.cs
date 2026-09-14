using Combat;
using Game;
using UnityEngine;

namespace CameraScripts
{
    [DefaultExecutionOrder(100)]
    public sealed class OrbitCamera : MonoBehaviour
    {
        private Transform _target;
        private float _pitch = 24;
        
        public float Yaw { get; private set; }
        
        [SerializeField] private float sensitivity = 0.12f;
        [SerializeField] private float distance = 5;

        public void Initialize(Transform follow)
        {
            _target = follow;
            Yaw = follow.eulerAngles.y;
        }
        
        public void Rotate(Vector2 mouseDelta)
        {
            Yaw += mouseDelta.x * sensitivity;
            _pitch = Mathf.Clamp(_pitch - mouseDelta.y * sensitivity, -15, 65);
        }
        
        private void LateUpdate()
        {
            if (!_target) return;
            
            Vector3 pivot = _target.position + Vector3.up * 1.45f;
            Quaternion rotation = Quaternion.Euler(_pitch, Yaw, 0);
            Vector3 backwards = rotation * Vector3.back;
            float actualDistance = distance;
            
            if (Physics.SphereCast(pivot, 0.2f, backwards, out var hit, distance,
                CombatPhysics.WorldMask, QueryTriggerInteraction.Ignore))
            {
                actualDistance = Mathf.Max(0.15f, hit.distance - 0.05f);
            }
            
            transform.SetPositionAndRotation(pivot + backwards * actualDistance, rotation);
        }
    }
}
