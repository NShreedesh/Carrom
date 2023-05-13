using System;
using Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UIClickSoundEffect : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField]
        private Button button;

        [Header("Button Audio")]
        [SerializeField]
        private AudioClip clickClip;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            AudioManager.Instance.PlaySoundFx(clickClip);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }
    }
}