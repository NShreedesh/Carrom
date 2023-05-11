using UnityEngine;

namespace Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private int[] score;

        private void Awake()
        {
            score = new int[GameManager.Instance.GetPlayerInGame()];
        }

        public int GetScore(int playerNumber) => score[playerNumber];

        public int SetScore(int playerNumber, int incrementScore)
        {
            int updatedScore = score[playerNumber] + incrementScore;
            if (updatedScore <= 0) return 0;
            score[playerNumber] = updatedScore;
            return score[playerNumber];
        } 
    }
}
