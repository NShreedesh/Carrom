using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Carom
{
    public class CaromSlider : MonoBehaviour
    {
        [Header("Slider")]
        [SerializeField]
        private Slider slider;

        public float GetSliderValue()
        {
            return slider.value;
        }
        
        public void ResetSliderValue()
        {
            slider.value = 0;
        }
    }
}
