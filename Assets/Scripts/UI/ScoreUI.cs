using System;
using Scripts.Manager;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private ScoreManager scoreManager;
        
        [Header("Score Values")]
        [SerializeField]
        private TMP_Text scoreText;
        [SerializeField]
        private int playerNumber;

        private void Awake()
        {
            ScoreManager.OnScoreUISet += SetScoreUI;
            ChangeScoreUIText(0);
        }

        private void SetScoreUI(int score)
        {
            if (GameManager.Instance.GetCurrentPlayerTurn() == playerNumber)
            {
                ChangeScoreUIText(score);
            }
        }

        private void ChangeScoreUIText(int score)
        {
            scoreText.text = $"{score.ToString()}/{scoreManager.GetTotalScore()}";
        }
        

        private void OnDisable()
        {
            ScoreManager.OnScoreUISet -= SetScoreUI;
        }
    }
}
