using System;
using Scripts.Manager;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private int playerNumber;

        private void Awake()
        {
            ScoreManager.OnScoreUISet += SetScoreUI;
        }

        private void SetScoreUI(int score, int totalScore)
        {
            if(GameManager.Instance.GetCurrentPlayerTurn() == playerNumber)
                scoreText.text = $"{score.ToString()}/{totalScore}";
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreUISet -= SetScoreUI;
        }
    }
}
