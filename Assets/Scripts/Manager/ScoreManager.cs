using System;
using UnityEngine;

namespace Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private int[] score;

        public static Action<int> OnScoreUISet;
        
        private void Awake()
        {
            score = new int[GameManager.Instance.GetPlayerInGame()];
        }

        public int GetScore(int playerNumber) => score[playerNumber];

        public void SetScore(int playerNumber, int incrementScore)
        {
            int updatedScore = score[playerNumber] + incrementScore;
            if (updatedScore <= 0)
            {
                score[playerNumber] = 0;
            }
            else
            {
                score[playerNumber] = updatedScore;
            }
            OnScoreUISet?.Invoke(score[playerNumber]);
        } 
    }
}
