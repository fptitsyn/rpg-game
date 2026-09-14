using Actors.Health;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class HealthBar : MonoBehaviour
    {
        private IHealth _health;
        private Image _fill;
        private TMP_Text _label;
        private Camera _viewCamera;
        
        public void Initialize(IHealth model, Image bar, TMP_Text text, Camera cam = null)
        {
            _health = model;
            _fill = bar;
            _label = text;
            _viewCamera = cam;
            _health.Changed += Refresh;
            Refresh(_health.Current, _health.Maximum);
        }
        
        private void Refresh(float current, float maximum)
        {
            _fill.fillAmount = current / maximum;
            _fill.color = Color.Lerp(new Color(0.9f, 0.2f, 0.2f), new Color(0.25f, 0.85f, 0.45f), current / maximum);
            if (_label != null) _label.text = $"HP  {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
        }
        
        private void LateUpdate()
        {
            if (_viewCamera)
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
