using Scripts.InputControls;
using UnityEngine;

namespace Scripts.Carom
{
    public class Striker : MonoBehaviour
    {
        [Header("Required Components")]
        [SerializeField]
        private InputController inputController;
        
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
        private CaromSlider caromSlider;
        [SerializeField]
        private float caromSliderSpeed = 30;

        private void Update()
        {
            ChangeStrikerWithSliderValue();
            ShootStriker();
        }

        private void ChangeStrikerWithSliderValue()
        {
            if(isStrikerShot) return;
            
            float strikerTargetPosition = caromSlider.GetSliderValue();
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
            else if(isDraggingStriker && !inputController.GetMousePress().WasPressedThisFrame())
            {
                rb.AddForce(-power * shootForce, ForceMode2D.Impulse);
                isDraggingStriker = false;
                isStrikerShot = true;
            }
        }
        
        public Vector2 GetPower() => power;

        public bool GetIsDragging() => isDraggingStriker;
    }
}