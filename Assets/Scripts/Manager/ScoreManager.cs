using System;
using Scripts.Enums;
using UnityEngine;

namespace Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField]
        private int[] score;
        [SerializeField]
        private int totalScoreToWin = 160;

        [Header("Coin Type Score")]
        [SerializeField]
        private int blackCoinScore = 10;
        [SerializeField]
        private int whiteCoinScore = 20;
        [SerializeField]
        private int redCoinScore = 50;
        [SerializeField]
        private int strikerCoinScore = -10;

        public static Action<int, int> OnScoreUISet;
        public static Action<int> OnTotalScoreReached;
        
        private void Awake()
        {
            score = new int[GameManager.Instance.GetPlayerInGame()];
        }

        public int GetCoinTypeScore(CoinType coinType)
        {
            return coinType switch
            {
                CoinType.White => whiteCoinScore,
                CoinType.Black => blackCoinScore,
                CoinType.Red => redCoinScore,
                CoinType.Striker => strikerCoinScore,
                _ => 0
            };
        }
        
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
            OnScoreUISet?.Invoke(score[playerNumber], totalScoreToWin);

            if (score[playerNumber] > totalScoreToWin)
                OnTotalScoreReached?.Invoke(playerNumber);
        } 
    }
}
