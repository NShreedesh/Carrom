using Scripts.Enums;
using Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        [Header("Button")]
        [SerializeField]
        private Button playButton;

        private void Awake()
        {
            gameOverPanel.SetActive(false);
            GameManager.OnGameStateChanged += OnGameOverTriggered;
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnGameOverTriggered(GameState gameState)
        {
            if (gameState == GameState.Play) return;
            
            gameOverPanel.SetActive(true);
            ChangeUIText(gameState);
        }

        private void ChangeUIText(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Win:
                    displayText.text = "Win";
                    buttonText.text = "Play Again!";
                    break;
                case GameState.Lose:
                    displayText.text = "Lose";
                    buttonText.text = "Try Again!";
                    break;
                case GameState.Pause:
                    displayText.text = "Pause";
                    buttonText.text = "Play!";
                    break;
            }
        }

        private void OnPlayButtonClicked()
        {
            switch (GameManager.Instance.GetGameState())
            {
                case GameState.Pause:
                    GameManager.Instance.SetGameState(GameState.Play);
                    gameOverPanel.SetActive(false);
                    break;
                case GameState.Win:
                case GameState.Lose:
                    GameManager.Instance.Reset();
                    GameManager.Instance.SetGameState(GameState.Play);
                    SceneManager.LoadScene(0);
                    break;
            }
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameOverTriggered;
            playButton.onClick.RemoveAllListeners();
        }
    }
}