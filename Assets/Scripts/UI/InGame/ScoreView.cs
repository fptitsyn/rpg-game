using Game;
using TMPro;
using UnityEngine;

namespace UI.InGame
{
    public sealed class ScoreView : MonoBehaviour
    {
        [SerializeField] TMP_Text scoreText;

        private Score _score;

        public void Bind(Score score)
        {
            _score = score;
            _score.Changed += Refresh;
            Refresh(_score.Value);
        }

        private void Refresh(int value)
        {
            scoreText.text = $"Score: {value}";
        }

        private void OnDestroy()
        {
            _score.Changed -= Refresh;
        }
    }
}