using System;
using Scripts.Enums;
using Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class GamePlayUI : MonoBehaviour
    {
        [SerializeField]
        private Button pauseButton;

        private void Awake()
        {
            pauseButton.onClick.AddListener(() =>
            {
                GameManager.Instance.SetGameState(GameState.Pause);
            });
        }

        private void OnDisable()
        {
            pauseButton.onClick.RemoveAllListeners();
        }
    }
}