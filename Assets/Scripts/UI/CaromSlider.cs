using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class CaromSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Slider")]
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private bool isSliderBeingUsed;

        public void OnPointerDown(PointerEventData eventData)
        {
            isSliderBeingUsed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isSliderBeingUsed = false;
        }

        public float GetSliderValue() => slider.value;
        
        public float GetSliderMinValue() => slider.minValue;
        
        public float GetSliderMaxValue() => slider.maxValue;
        
        public bool GetIsSliderBeingUsed() => isSliderBeingUsed;
        
        public float SetSliderValue(float value) => slider.value = value;

        public void ResetSliderValue() => slider.value = 0;

        public void DisableSlider() => slider.interactable = false;

        public void EnableSlider() => slider.interactable = true;
    }
}
