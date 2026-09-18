using Actors.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text label;
        [SerializeField] private bool faceCamera;

        private IHealth _health;
        private Camera _viewCamera;

        public void Bind(IHealth healthModel, Camera cam = null)
        {
            if (_health != null)
            {
                _health.Changed -= Refresh;
            }

            _health = healthModel;
            _viewCamera = cam;

            if (_health == null)
            {
                return;
            }

            _health.Changed += Refresh;
            Refresh(_health.Current, _health.Maximum);
        }

        private void Refresh(float current, float maximum)
        {
            if (fill == null)
            {
                return;
            }

            float normalizedHealth = current / maximum;

            fill.fillAmount = normalizedHealth;
            fill.color = Color.Lerp(
                new Color(0.9f, 0.2f, 0.2f),
                new Color(0.25f, 0.85f, 0.45f),
                normalizedHealth
            );

            if (label != null)
            {
                label.text = $"HP {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
            }
        }

        private void LateUpdate()
        {
            if (faceCamera && _viewCamera)
            {
                transform.rotation = _viewCamera.transform.rotation;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Changed -= Refresh;
            }
        }
    }
}