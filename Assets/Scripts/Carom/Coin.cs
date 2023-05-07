using System.Collections;
using UnityEngine;

namespace Scripts.Carom
{
    public class Coin : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb;

        public void MakeInHole(Transform target)
        {
            rb.Sleep();
            transform.position = target.position;
            StartCoroutine(TakeToHole(target));
        }

        private IEnumerator TakeToHole(Transform target)
        {
            while (transform.position != target.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 20);
                yield return null;
            }
        }
    }
}
