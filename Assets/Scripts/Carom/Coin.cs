using System;
using System.Collections;
using Scripts.Extensions;
using Scripts.Interfaces;
using Scripts.Manager;
using UnityEngine;

namespace Scripts.Carom
{
    public class Coin : MonoBehaviour, IHitEffect
    {
        [Header("Components")]
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        [Header("Coin")]
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private new Collider2D collider;

        [Header("Enter Hole")]
        [SerializeField]
        private float holeEnterSpeed = 10;
        [SerializeField]
        private Vector3 coinInHoleScale;

        [Header("Audio")]
        [SerializeField]
        private AudioClip strikeHitClip;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Hole coinInHole))
            {
                MakeInHole(coinInHole.transform);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(rb.velocity.magnitude < 0.1f) return;
            if(!other.gameObject.TryGetComponent(out IHitEffect hitEffect))  return;
            AudioManager.Instance.PlaySoundFx(strikeHitClip);
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
            TryGetComponent(out Striker striker);
            striker?.SetCanResetStriker(false);
            
            spriteRenderer.ChangeAlpha(0.5f);
            transform.localScale = coinInHoleScale;
            while (Math.Abs(transform.position.x - target.position.x) != 0)
            {
                rb.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * holeEnterSpeed);
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
            striker?.SetCanResetStriker(true);
            yield return new WaitForSeconds(2);
            if (!striker)
            {
                gameObject.SetActive(false);
            }
        }

        public float GetVelocity()
        {
            return rb.velocity.magnitude;
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
