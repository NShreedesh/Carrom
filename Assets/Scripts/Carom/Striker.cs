using Scripts.Enums;
using Scripts.Extensions;
using Scripts.InputControls;
using Scripts.Interfaces;
using Scripts.Manager;
using Scripts.UI;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.WSA;

namespace Scripts.Carom
{
    public class Striker : MonoBehaviour, IHitEffect
    {
        [Header("Required Components")]
        [SerializeField]
        private InputController inputController;
        [SerializeField]
        private CircleCollider2D collider;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [Header(("Raycast"))]
        [SerializeField]
        private Camera camera;
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
        [SerializeField]
        private float powerThreshold = 1f;

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
        private Transform[] pockets;
        [SerializeField]
        private float strikerRadius = 0.2f;

        [Header("Bot Strike")]
        private BotStrikeData[] _botStrikeData;

        [Header("Left")]
        [SerializeField]
        private bool isMovingLeft;

        private void Start()
        {
            _botStrikeData = new BotStrikeData[coins.Length];

            EnableDisableSlider();

            strikerDefaultScale = transform.localScale;
            collider.isTrigger = true;
        }

        private void Update()
        {
            ResetStriker();

            if (GameManager.Instance.GetGameState() != GameState.Play) return;
            ChangeStrikerWithSliderValue();
            if(GameManager.Instance.GetPlayerType() == PlayerType.Bot)
            {
                TryBotStrike();
            }
            else
            {
                ShootStriker();
            }
            CheckIfStrikerCollidedWithCoin();
        }

        private void CheckIfStrikerCollidedWithCoin()
        {
            if (isDraggingStriker) return;
            CaromSlider caromSlider = caromSliders[GameManager.Instance.GetCurrentPlayerTurn()];
            if (caromSlider.GetIsSliderBeingUsed()) return;
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
            if (isStrikerShot) return;

            float strikerTargetPosition = caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].GetSliderValue();
            Vector3 strikerCurrentPosition = transform.localPosition;
            strikerCurrentPosition.x = Mathf.MoveTowards(strikerCurrentPosition.x, strikerTargetPosition, caromSliderSpeed * Time.deltaTime);
            transform.localPosition = strikerCurrentPosition;
        }

        [ContextMenu("Bot Strike")]
        private void TryBotStrike()
        {
            if (isStrikerShot) return;

            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                if (!coins[i].gameObject.activeSelf) continue;

                _botStrikeData[i] = new();
                for (int j = 0; j <= 1; j++)
                {
                    Coin selectedCoin = coins[i];
                    Vector2 pocketPos = pockets[j].position;
                    Vector2 piecePos = selectedCoin.transform.position;

                    Vector2 distanceVector = pocketPos - piecePos;
                    Vector2 direction = distanceVector.normalized;
                    Vector2 impactPoint = piecePos - (direction * (strikerRadius + 0.1602883f));

                    _botStrikeData[i].direction[j] = direction;
                    _botStrikeData[i].impactPoints[j] = impactPoint;

                    Vector2 previousPosition = piecePos;

                    for (float d = 0; d <= distanceVector.magnitude; d += 0.1602883f)
                    {
                        Vector2 pos = previousPosition + direction * d;
                        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.1602883f, coinLayerMask);
                        if (results.Length == 0) _botStrikeData[i].point += 1;

                        foreach (Collider2D c in results)
                        {
                            if (c.gameObject == selectedCoin.gameObject) continue; 
                            _botStrikeData[i].point -= 1;
                        }
                    }

                    previousPosition = transform.position;
                    distanceVector = impactPoint - new Vector2(transform.position.x, transform.position.y);
                    direction = distanceVector.normalized;
                    for (float d = 0; d <= distanceVector.magnitude; d += strikerRadius)
                    {
                        Vector2 pos = previousPosition + direction * d;
                        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.1602883f, coinLayerMask);
                        if (results.Length == 0) _botStrikeData[i].point += 1;

                        foreach (Collider2D c in results)
                        {
                            if (c.gameObject == selectedCoin.gameObject) continue;
                            _botStrikeData[i].point -= 1;
                        }
                    }
                }
            }

            // TODO: striker with more points takes the shot from there with calculated power
            BotStrikeData botStrikeData = _botStrikeData.OrderByDescending(data => data.point).FirstOrDefault();
            if (botStrikeData != null)
            {
                Debug.Log(botStrikeData);
                Vector2 direction = botStrikeData.impactPoints[0] - new Vector2(transform.position.x, transform.position.y);
                direction.Normalize();
                Launch(new Vector2(0,1));
                isStrikerShot = true;
                canResetStriker = true;
                collider.isTrigger = false;
            }
        }

        private void ShootStriker()
        {
            if (isStrikerShot) return;

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
                if (power.magnitude > powerThreshold)
                {
                    Launch(-power);
                    caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].DisableSlider();
                    isDraggingStriker = false;
                    isStrikerShot = true;
                    canResetStriker = true;
                    collider.isTrigger = false;
                }
                else
                {
                    isDraggingStriker = false;
                }
            }
        }

        private void Launch(Vector2 power)
        {
            rb.AddForce(power * shootForce, ForceMode2D.Impulse);
        }

        private void ResetStriker()
        {
            if (rb.linearVelocity.magnitude > 0.02f) return;
            if (!canResetStriker) return;

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

            if (_botStrikeData == null) return;
            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                if (_botStrikeData[i] != null && _botStrikeData[i].impactPoints != null)
                {
                    for (int j = 0; j < _botStrikeData[i].impactPoints.Length; j++)
                    {
                        Vector2 point = _botStrikeData[i].impactPoints[j];
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(point, .05f);
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(point, point + _botStrikeData[i].direction[j]);
                    }
                }
            }

        }
#endif
        #endregion
    }

    [Serializable]
    public class BotStrikeData
    {
        public Vector2[] impactPoints = new Vector2[2];
        public Vector2[] direction = new Vector2[2];
        public int point = -1;
    }
}