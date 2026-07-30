using System;
using UnityEngine;

namespace Scripts.Carom
{
    public class StrikerPowerDisplay : MonoBehaviour
    {
        [Header("Scale")]
        [SerializeField]
        private float scaleUpTo = 7;

        public void SetStrikerPowerDisplay(Vector2 power)
        {
            float maxPower = Mathf.Max(Mathf.Abs(power.x), Mathf.Abs(power.y));
            transform.localScale = new Vector2(maxPower, maxPower) * scaleUpTo;
        }

        public void Reset()
        {

            transform.localScale = Vector2.zero;
        }
    }
}