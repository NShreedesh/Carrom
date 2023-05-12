using Scripts.Enums;
using Scripts.Manager;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Game Over UI")]
        [SerializeField]
        private GameObject gameOverPanel;
        [SerializeField]
        private TMP_Text displayText;
        [SerializeField]
        private TMP_Text buttonText;

        private void Awake()
        {
            gameOverPanel.SetActive(false);
            GameManager.OnGameWon += OnGameOverTriggered;
        }

        private void OnGameOverTriggered(int wonPlayer)
        {
            gameOverPanel.SetActive(true);
            ChangeUIText();
        }

        private void ChangeUIText()
        {
            if (GameManager.Instance.GetGameState() == GameState.Win)
            {
                displayText.text = "Win";
                buttonText.text = "Play Again!";
            }
            else if(GameManager.Instance.GetGameState() == GameState.Lose)
            {
                displayText.text = "Lose";
                buttonText.text = "Try Again!";
            }
            else if(GameManager.Instance.GetGameState() == GameState.Pause)
            {
                displayText.text = "Pause";
                buttonText.text = "Play!";
            }
        }

        private void OnDisable()
        {
            GameManager.OnGameWon -= OnGameOverTriggered;
        }
    }
}