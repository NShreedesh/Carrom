using System;
using Scripts.Enums;
using UnityEngine;

namespace Scripts.Manager
{
    public class GameManager : MonoBehaviour
    {
        [field: Header("Instance")]
        public static GameManager Instance { get; private set; }

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

        public static Action<int> OnGameWon;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            ScoreManager.OnTotalScoreReached += SetPlayerWinLoseState;
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
            OnGameWon?.Invoke(wonPlayer);
        }

        public GameState GetGameState() => gameState;

        private void SetGameState(GameState state)
        {
            gameState = state;
        }
    }
}
