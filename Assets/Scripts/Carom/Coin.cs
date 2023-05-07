using UnityEngine;

namespace Scripts.Carom
{
    public class Coin : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb;

        public void SleepRigidbody()
        {
            rb.Sleep();
        }
    }
}
