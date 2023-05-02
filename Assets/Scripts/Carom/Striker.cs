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
        private bool isDraggingStriker;
        [SerializeField]
        private Vector2 startMousePosition;
        [SerializeField]
        private Vector2 endMousePosition;

        private void Update()
        {
            Vector3 worldMousePosition = camera.ScreenToWorldPoint(inputController.GetMousePosition());
            RaycastHit2D hitInfo = Physics2D.Raycast(camera!.transform.position, 
                worldMousePosition,
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
                endMousePosition = worldMousePosition;
            }
            else if(isDraggingStriker && !inputController.GetMousePress().WasPressedThisFrame())
            {
                Vector2 force = endMousePosition - startMousePosition;
                rb.AddForce(-force * shootForce, ForceMode2D.Impulse);
                isDraggingStriker = false;
            }
        }
    }
}