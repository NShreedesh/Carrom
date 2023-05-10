using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Manager
{
    public class GameManager : MonoBehaviour
    {
        [field: Header("Instance")]
        public static GameManager Instance { get; private set; }

        [FormerlySerializedAs("howManyPlayerGame")]
        [Header("Values")]
        [SerializeField]
        [Range(1, 4)]
        private int playerInGame = 2;
        [SerializeField]
        private int currentPlayerTurn = 0;

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
        }
        
        public int GetCurrentPlayerTurn() => currentPlayerTurn;

        public void SetCurrentPlayerTurn()
        {
            currentPlayerTurn = (++currentPlayerTurn) % playerInGame;
        }
    }
}
