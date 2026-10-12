using Actors.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class ManaBar : MonoBehaviour
    {
        [SerializeField] Image fill;

        private Mana _mana;

        public void Bind(Mana mana)
        {
            _mana = mana;
            _mana.Changed += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            fill.fillAmount = _mana.Current / _mana.Maximum;
        }

        private void OnDestroy()
        {
            _mana.Changed -= Refresh;
        }
    }
}