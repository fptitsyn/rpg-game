using UnityEngine;

namespace InfinityPBR
{
    public class BoneMotionReaction : MonoBehaviour
    {
        public Transform parentBone; // The parent bone
        public Rigidbody characterRigidbody; // The Rigidbody of the character
        public float sensitivity = 2.0f; // Adjust to change how much the tail responds to movement
        public bool effectEnabled = true; // Toggle to turn the effect on/off
        public AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default to a smooth curve

        private Transform[] affectedBones;
        private Quaternion[] defaultLocalRotations;
        private Quaternion lastLocalRotation;
        private Quaternion lastGlobalRotation;

        void Start()
        {
            // Populate the affectedBones array with the parent bone and its children
            affectedBones = parentBone.GetComponentsInChildren<Transform>();
            
            // Store the default local rotations of the affected bones
            defaultLocalRotations = new Quaternion[affectedBones.Length];
            for (int i = 0; i < affectedBones.Length; i++)
            {
                defaultLocalRotations[i] = affectedBones[i].localRotation;
            }

            lastLocalRotation = transform.localRotation;
            lastGlobalRotation = transform.rotation;
        }

        void LateUpdate()
        {
            if (!effectEnabled) return;
            if (characterRigidbody != null) 
            {
                UpdateWithRigidbody();
            } 
            else 
            {
                UpdateWithoutRigidbody();
            }
            lastLocalRotation = transform.localRotation;
            lastGlobalRotation = transform.rotation;
        }

        void UpdateWithRigidbody()
        {
            float angularVelocity = characterRigidbody.angularVelocity.magnitude;
            float rotationChange = Quaternion.Angle(transform.rotation, lastGlobalRotation) * Mathf.Sign(Quaternion.Dot(transform.rotation, lastGlobalRotation));
            ApplyBoneRotations(angularVelocity, rotationChange);
        }

        void UpdateWithoutRigidbody()
        {
            float angularVelocity = Quaternion.Angle(transform.localRotation, lastLocalRotation) / Time.deltaTime;
            float rotationChange = Quaternion.Angle(transform.rotation, lastGlobalRotation) * Mathf.Sign(Quaternion.Dot(transform.rotation, lastGlobalRotation));
            ApplyBoneRotations(angularVelocity, rotationChange);
        }

        void ApplyBoneRotations(float angularVelocity, float rotationChange)
        {
            for (int i = 0; i < affectedBones.Length; i++)
            {
                // Calculate the scaled sensitivity for this bone based on the curve
                float curveValue = motionCurve.Evaluate((float)i / (affectedBones.Length - 1));
                float scaledSensitivity = sensitivity * curveValue;
                
                Quaternion additionalRotation = Quaternion.Euler(0, 0, -rotationChange + scaledSensitivity * angularVelocity);
                
                // Interpolate towards the desired local rotation
                Quaternion targetLocalRotation = defaultLocalRotations[i] * additionalRotation;
                affectedBones[i].localRotation = Quaternion.Lerp(affectedBones[i].localRotation, targetLocalRotation, Time.deltaTime);
            }
        }
    }
}
