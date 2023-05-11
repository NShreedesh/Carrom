using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class CaromSlider : MonoBehaviour
    {
        [Header("Slider")]
        [SerializeField]
        private Slider slider;

        public float GetSliderValue() => slider.value;

        public void ResetSliderValue() => slider.value = 0;

        public void DisableSlider() => slider.interactable = false;

        public void EnableSlider() => slider.interactable = true;
    }
}
