using System;
using Helpers;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Values")]
        [SerializeField]
        [Range(1, 4)]
        private int playerInGame = 2;
        [SerializeField]
        private int currentPlayerTurn;
        [SerializeField]
        private bool isHoled;
        [SerializeField]
        private int wonPlayer = -1;
        [SerializeField]
        private GameState gameState;

        public static Action<GameState> OnGameStateChanged;

        private void OnEnable()
        {
            ScoreManager.OnTotalScoreReached += SetPlayerWinLoseState;
        }

        private void OnDisable()
        {
            ScoreManager.OnTotalScoreReached -= SetPlayerWinLoseState;
        }

        public int GetCurrentPlayerTurn() => currentPlayerTurn;
        
        public int GetPlayerInGame() => playerInGame;
        
        public bool SetIsHoled(bool value) => isHoled = value;
        
        public void SetCurrentPlayerTurn()
        {
            if (isHoled)
            {
                isHoled = false;
                return;
            }
            currentPlayerTurn = (++currentPlayerTurn) % playerInGame;
        }

        public int GetWonPlayer() => wonPlayer;

        private void SetPlayerWinLoseState(int value)
        {
            wonPlayer = value;
            SetGameState(wonPlayer == 0 ? GameState.Win : GameState.Lose);
        }

        public GameState GetGameState() => gameState;

        public void SetGameState(GameState state)
        {
            gameState = state;
            OnGameStateChanged?.Invoke(gameState);
        }

        public void Reset()
        {
            isHoled = false;
            currentPlayerTurn = 0;
            wonPlayer = -1;
        }
    }
}
