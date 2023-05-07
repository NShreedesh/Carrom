using System.Collections;
using UnityEngine;

namespace Scripts.Carom
{
    public class Coin : MonoBehaviour
    {
        [Header("Coin")]
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private new Collider2D collider;

        [Header("Enter Hole")]
        [SerializeField]
        private float holeEnterSpeed = 2;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out CoinInHole coinInHole))
            {
                MakeInHole(coinInHole.transform);
            }
        }
        
        private void MakeInHole(Transform target)
        {
            collider.enabled = false;
            rb.bodyType = RigidbodyType2D.Static;
            rb.Sleep();
            StartCoroutine(TakeToHole(target));
        }

        private IEnumerator TakeToHole(Transform target)
        {
            while (transform.position != target.position)
            {
                rb.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * holeEnterSpeed);
                yield return null;
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
