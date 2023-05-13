using System.Linq;
using Scripts.Enums;
using Scripts.Extensions;
using Scripts.InputControls;
using Scripts.Interfaces;
using Scripts.Manager;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Carom
{
    public class Striker : MonoBehaviour, IHitEffect
    {
        [Header("Required Components")]
        [SerializeField]
        private InputController inputController;
        [SerializeField]
        private new CircleCollider2D collider;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        [Header(("Raycast"))]
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        private LayerMask strikerLayerMask;

        [Header("Physics")]
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private float shootForce;

        [Header("Shooting Carom")]
        [SerializeField]
        private Vector3 strikerDefaultScale;
        [SerializeField]
        private bool isStrikerShot;
        [SerializeField]
        private bool isDraggingStriker;
        [SerializeField]
        private Vector2 startMousePosition;
        [SerializeField]
        private Vector2 endMousePosition;
        [SerializeField]
        private Vector2 power;
        
        [Header("Slider")]
        [SerializeField]
        private CaromSlider[] caromSliders;
        [SerializeField]
        private float caromSliderSpeed = 30;

        [Header("Reset Striker")]
        [SerializeField]
        private LayerMask coinLayerMask;
        [SerializeField]
        private Vector3[] playerStrikerPositions;
        [SerializeField]
        private bool canResetStriker;
        [SerializeField]
        private Coin[] coins;
        [SerializeField]
        private float strikerRadius = 0.2f;

        [Header("Left")]
        [SerializeField]
        private bool isMovingLeft;
        
        private void Start()
        {
            EnableDisableSlider();
            
            strikerDefaultScale = transform.localScale;
            collider.isTrigger = true;
        }

        private void Update()
        {
            ResetStriker();
            
            if(GameManager.Instance.GetGameState() != GameState.Play) return;
            ChangeStrikerWithSliderValue();
            ShootStriker();
            CheckIfStrikerCollidedWithCoin();
        }

        private void CheckIfStrikerCollidedWithCoin()
        {
             if (isDraggingStriker) return;
             CaromSlider caromSlider = caromSliders[GameManager.Instance.GetCurrentPlayerTurn()];
             if(caromSlider.GetIsSliderBeingUsed()) return;
             if (isMovingLeft && transform.position.x <= caromSlider.GetSliderMinValue())
             {
                 isMovingLeft = false;
             }

             Collider2D hitInfo = Physics2D.OverlapCircle(transform.position, strikerRadius, coinLayerMask);
             if (hitInfo is null) return;
             if (!hitInfo.TryGetComponent(out Coin coin)) return;

             switch (isMovingLeft)
             {
                 case true when transform.position.x <= caromSlider.GetSliderMinValue():
                 {
                     float resetPosition = transform.position.x + coin.GetCollider().bounds.extents.x;
                     caromSlider.SetSliderValue(resetPosition);
                     isMovingLeft = false;
                     break;
                 }
                 case false:
                 {
                     float resetPosition = transform.position.x + coin.GetCollider().bounds.extents.x;
                     caromSlider.SetSliderValue(resetPosition);
                     break;
                 }
             }

             if (caromSlider.GetSliderValue() >= caromSlider.GetSliderMaxValue())
             {
                 caromSlider.SetSliderValue(caromSlider.GetSliderMinValue());
                 isMovingLeft = true;
             }
        }

        private void ChangeStrikerWithSliderValue()
        {
            if(isStrikerShot) return;
            
            float strikerTargetPosition = caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].GetSliderValue();
            Vector3 strikerCurrentPosition = transform.localPosition;
            strikerCurrentPosition.x = Mathf.MoveTowards(strikerCurrentPosition.x, strikerTargetPosition, caromSliderSpeed * Time.deltaTime);
            transform.localPosition = strikerCurrentPosition;
        }
        
        private void ShootStriker()
        {
            if(isStrikerShot) return;
            
            Vector3 worldMousePosition = camera.ScreenToWorldPoint(inputController.GetMousePosition());
            RaycastHit2D hitInfo = Physics2D.Raycast(worldMousePosition,
                Vector3.forward,
                10,
                strikerLayerMask);

            if (!isDraggingStriker && inputController.GetMousePress().WasPressedThisFrame())
            {
                if (hitInfo.collider is null) return;
                startMousePosition = worldMousePosition;
                isDraggingStriker = true;
            }
            else if (inputController.GetMousePress().IsPressed())
            {
                power = endMousePosition - startMousePosition;
                power.x = Mathf.Clamp(power.x, -1, 1);
                power.y = Mathf.Clamp(power.y, -1, 1);
                endMousePosition = worldMousePosition;
            }
            else if (isDraggingStriker && !inputController.GetMousePress().WasPressedThisFrame())
            {
                rb.AddForce(-power * shootForce, ForceMode2D.Impulse);
                caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].DisableSlider();
                isDraggingStriker = false;
                isStrikerShot = true;
                canResetStriker = true;
                collider.isTrigger = false;
            }
        }

        private void ResetStriker()
        {
            if (rb.velocity.magnitude > 0.02f) return;
            if(!canResetStriker) return;

            if (coins.Any(coin => coin.GetVelocity() > 0)) return;

            GameManager.Instance.SetCurrentPlayerTurn();
            caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].EnableSlider();
            caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].ResetSliderValue();
            SetStrikerPosition();
            spriteRenderer.ChangeAlpha(1);
            canResetStriker = false;
            collider.enabled = true;
            collider.isTrigger = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            transform.localScale = strikerDefaultScale;
            isStrikerShot = false;
            isMovingLeft = false;
        }

        private void EnableDisableSlider()
        {
            foreach (CaromSlider slider in caromSliders)
            {
                slider.DisableSlider();
            }
            caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].EnableSlider();
        }

        private void SetStrikerPosition()
        {
            transform.localPosition = playerStrikerPositions[GameManager.Instance.GetCurrentPlayerTurn()];
        }
        
        public Vector2 GetPower() => power;

        public bool GetIsDragging() => isDraggingStriker;

        public bool SetCanResetStriker(bool value) => canResetStriker = value;

        #region  Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, strikerRadius);
        }
#endif
        #endregion
    }
}