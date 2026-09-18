using Actors;
using Actors.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.InGame
{
    public sealed class GameHud : MonoBehaviour
    {
        [SerializeField] private Image magicCooldownFill;
        [SerializeField] private TMP_Text magicCooldownText;
        [SerializeField] private GameObject deathPanel;

        private IHealth _playerHealth;
        private ActorCombat _playerCombat;

        public void Bind(IHealth health, ActorCombat combat)
        {
            if (_playerHealth != null)
            {
                _playerHealth.Died -= ShowDeathPanel;
            }

            _playerHealth = health;
            _playerCombat = combat;

            if (_playerHealth != null)
            {
                _playerHealth.Died += ShowDeathPanel;
            }

            if (deathPanel != null)
            {
                deathPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (!_playerCombat)
            {
                return;
            }

            float remaining = _playerCombat.MagicRemaining;
            float cooldown = _playerCombat.MagicCooldown;

            if (magicCooldownFill)
            {
                magicCooldownFill.fillAmount = cooldown > 0 ? remaining / cooldown : 0;
            }

            if (magicCooldownText)
            {
                magicCooldownText.text = remaining > 0 ? $"Magic {remaining:0.0}s" : "Magic Ready";
            }
        }

        private void ShowDeathPanel()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.Died -= ShowDeathPanel;
            }
        }
    }
}