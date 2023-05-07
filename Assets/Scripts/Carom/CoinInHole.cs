using System;
using UnityEngine;

namespace Scripts.Carom
{
    public class CoinInHole : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Coin coin))
            {
                coin.SleepRigidbody();
            }
        }
    }
}