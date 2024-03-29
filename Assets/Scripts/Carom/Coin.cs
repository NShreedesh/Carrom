using System;
using System.Collections;
using Scripts.Enums;
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
        private ScoreManager scoreManager;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [Header("Physics")]
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private new CircleCollider2D collider;

        [Header("Coin")]
        [SerializeField]
        private CoinType coinType;

        [Header("Enter Hole")]
        [SerializeField]
        private float holeEnterSpeed = 10;
        [SerializeField]
        private Vector3 coinInHoleScale;

        [Header("Audio")]
        [SerializeField]
        private AudioClip strikeHitClip;
        [SerializeField]
        private AudioClip inHoleClip;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Hole coinInHole))
            {
                MakeInHole(coinInHole.transform);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(GetVelocity() < 0.1f) return;
            if(!other.gameObject.TryGetComponent(out IHitEffect _))  return;
            float volume = Mathf.Clamp(GetVelocity() / 25, 0, 1);
            AudioManager.Instance.PlaySoundFx(strikeHitClip, volume);
        }

        private void MakeInHole(Transform target)
        {
            collider.enabled = false;
            rb.bodyType = RigidbodyType2D.Static;
            rb.Sleep();
            StartCoroutine(TakeToHole(target));
            UpdateScore();
            AudioManager.Instance.PlaySoundFx(inHoleClip);
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
            GameManager.Instance.SetIsHoled(!striker);
            yield return new WaitForSeconds(1);
            striker?.SetCanResetStriker(true);
            yield return new WaitForSeconds(2);
            if (!striker) gameObject.SetActive(false);
        }

        private void UpdateScore()
        {
            scoreManager.SetScore(GameManager.Instance.GetCurrentPlayerTurn(), scoreManager.GetCoinTypeScore(coinType));
        }

        public float GetVelocity() => rb.velocity.magnitude;

        public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
        
        public CircleCollider2D GetCollider() => collider;
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
