using Actors;
using Actors.Health;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public sealed class GameHud : MonoBehaviour
    {
        private IHealth _health;
        private ActorCombat _combat;
        private Image _cooldown;
        private Text _cooldownText;
        private GameObject _deathPanel;
        
        public void Initialize(IHealth model, ActorCombat attacks, Image magicFill, Text magicText, GameObject gameOver)
        {
            _health = model;
            _combat = attacks;
            _cooldown = magicFill;
            _cooldownText = magicText;
            _deathPanel = gameOver;
            _health.Died += OnDeath;
            _deathPanel.SetActive(false);
        }
        
        private void Update()
        {
            float remaining = _combat.MagicRemaining;
            _cooldown.fillAmount = remaining / _combat.MagicCooldown;
            _cooldownText.text = remaining > 0 ? $"MAGIC  {remaining:0.0}s" : "MAGIC  READY";
        }
        
        private void OnDeath()
        {
            _deathPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        private void OnDestroy() { if (_health != null) _health.Died -= OnDeath; }
    }
}
