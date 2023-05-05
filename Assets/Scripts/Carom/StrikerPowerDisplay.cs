using System;
using UnityEngine;

namespace Scripts.Carom
{
    public class StrikerPowerDisplay : MonoBehaviour
    {
        [Header("Striker")]
        [SerializeField]
        private Striker striker;

        [Header("Scale")]
        [SerializeField]
        private float scaleUpTo = 7;

        private void Update()
        {
            if (!striker.GetIsDragging())
            {
                transform.localScale = Vector2.zero;
                return;
            }
            
            Vector2 power = striker.GetPower();
            float maxPower = Mathf.Max(Mathf.Abs(power.x), Mathf.Abs(power.y));
            transform.localScale = new Vector2(maxPower, maxPower) * scaleUpTo;
        }
    }
}