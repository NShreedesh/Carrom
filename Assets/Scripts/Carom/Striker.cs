using Scripts.Enums;
using Scripts.Extensions;
using Scripts.InputControls;
using Scripts.Interfaces;
using Scripts.Manager;
using Scripts.UI;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
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
        [SerializeField]
        private StrikerPowerDisplay strikerPowerDisplay;
        [SerializeField]
        private StrikerArrowDisplay strikerArrowDisplay;

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
        [SerializeField]
        private BotStrikeData botStrikeDataResult;
        [SerializeField]
        private bool shouldShowAllBotStrikeData;
        private BotStrikeData[] _botStrikeData;
        private int _maxPointIndex;

        [Header("Left")]
        [SerializeField]
        private bool isMovingLeft;

        private void Start()
        {
            _botStrikeData = new BotStrikeData[coins.Length];
            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                _botStrikeData[i] = new();
            }

            EnableDisableSlider();

            strikerDefaultScale = transform.localScale;
            collider.isTrigger = true;
        }

        private async void Update()
        {
            ResetStriker();

            if (GameManager.Instance.GetGameState() != GameState.Play) return;
            ChangeStrikerWithSliderValue();
            if (GameManager.Instance.GetPlayerType() == PlayerType.Bot)
            {
                try
                {
                    await TryBotStrike();
                }
                catch (Exception)
                {
                }
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

        private async Awaitable TryBotStrike()
        {
            if (isStrikerShot) return;
            isStrikerShot = true;

            int pocketToUse = 2;

            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                _botStrikeData[i] = new()
                {
                    direction = new Vector2[pocketToUse],
                    impactPoints = new Vector2[pocketToUse],
                    points = new int[pocketToUse]
                };
                for (int j = 0; j < _botStrikeData[i].points.Length; j++)
                {
                    _botStrikeData[i].points[j] = int.MinValue;
                }
            }

            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                if (coins[i].IsHoled) continue;

                for (int j = 0; j < pocketToUse; j++)
                {
                    Coin selectedCoin = coins[i];
                    Vector2 pocketPos = pockets[j].position;
                    Vector2 piecePos = selectedCoin.transform.position;

                    Vector2 distanceVector = pocketPos - piecePos;
                    Vector2 direction = distanceVector.normalized;
                    Vector2 impactPoint = piecePos - (direction * (strikerRadius + 0.1602883f));

                    _botStrikeData[i].direction[j] = direction;
                    _botStrikeData[i].impactPoints[j] = impactPoint; 
                    _botStrikeData[i].points[j] = 0;

                    Vector2 previousPosition = piecePos;

                    for (float d = 0; d <= distanceVector.magnitude; d += 0.1602883f)
                    {
                        Vector2 pos = previousPosition + direction * d;
                        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.1602883f, coinLayerMask);

                        foreach (Collider2D c in results)
                        {
                            if (c.gameObject == selectedCoin.gameObject) continue;
                            _botStrikeData[i].points[j] -= 1;
                        }
                    }

                    previousPosition = transform.position;
                    distanceVector = impactPoint - new Vector2(transform.position.x, transform.position.y);
                    direction = distanceVector.normalized;
                    for (float d = 0; d <= distanceVector.magnitude; d += strikerRadius)
                    {
                        Vector2 pos = previousPosition + direction * d;
                        Collider2D[] results = Physics2D.OverlapCircleAll(pos, 0.1602883f, coinLayerMask);

                        foreach (Collider2D c in results)
                        {
                            if (c.gameObject == selectedCoin.gameObject) continue;
                            _botStrikeData[i].points[j] -= 1;
                        }
                    }
                }
            }

            await Awaitable.WaitForSecondsAsync(1f, destroyCancellationToken);

            botStrikeDataResult = null;
            _maxPointIndex = -1;
            int maxPointValue = int.MinValue;

            for (int i = 0; i < _botStrikeData.Length; i++)
            {
                BotStrikeData data = _botStrikeData[i];

                for (int j = 0; j < data.points.Length; j++)
                {
                    if (data.points[j] > maxPointValue)
                    {
                        maxPointValue = data.points[j];
                        botStrikeDataResult = data;
                        _maxPointIndex = j;
                    }
                }
            }

            if (botStrikeDataResult != null)
            {
                Vector2 direction = botStrikeDataResult.impactPoints[_maxPointIndex] - new Vector2(transform.position.x, transform.position.y);
                direction.Normalize();

                float time = 0;
                float duration = 0.3f;
                Vector2 startDirection = Vector2.zero;
                Vector2 currentDirection = Vector2.zero;
                while (time < duration)
                {
                    time += Time.deltaTime;
                    currentDirection = Vector2.Lerp(startDirection, direction, time / duration);
                    strikerPowerDisplay.SetStrikerPowerDisplay(-currentDirection);
                    strikerArrowDisplay.UpdateStrikerArrowDisplay(-currentDirection);
                    await Awaitable.EndOfFrameAsync();
                }

                currentDirection = direction;
                strikerPowerDisplay.SetStrikerPowerDisplay(-currentDirection);
                strikerArrowDisplay.UpdateStrikerArrowDisplay(-currentDirection);

                await Awaitable.WaitForSecondsAsync(1f, destroyCancellationToken);

                Launch(direction);

                strikerPowerDisplay.Reset();
                strikerArrowDisplay.Reset();

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
                if (isDraggingStriker)
                {
                    strikerPowerDisplay.SetStrikerPowerDisplay(power);
                    strikerArrowDisplay.UpdateStrikerArrowDisplay(power);
                }
            }
            else if (isDraggingStriker && !inputController.GetMousePress().WasPressedThisFrame())
            {
                if (power.magnitude > powerThreshold)
                {
                    strikerPowerDisplay.Reset();
                    strikerArrowDisplay.Reset();

                    Launch(-power);
                    caromSliders[GameManager.Instance.GetCurrentPlayerTurn()].DisableSlider();
                    isDraggingStriker = false;
                    isStrikerShot = true;
                    canResetStriker = true;
                    collider.isTrigger = false;
                }
                else
                {
                    strikerPowerDisplay.Reset();
                    strikerArrowDisplay.Reset();

                    isDraggingStriker = false;
                }
            }
        }

        private void Launch(Vector2 direction)
        {
            rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
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

            if (shouldShowAllBotStrikeData)
            {
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

            for (int i = 0; i < botStrikeDataResult.impactPoints.Length; i++)
            {
                Vector2 point = botStrikeDataResult.impactPoints[i];
                Gizmos.color = i == _maxPointIndex ? Color.green : Color.red;
                Gizmos.DrawWireSphere(point, .05f);
                Gizmos.DrawLine(point, point + botStrikeDataResult.direction[i]);
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
        public int[] points = new int[2];
    }
}