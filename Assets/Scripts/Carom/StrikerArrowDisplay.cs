using UnityEngine;

namespace Scripts.Carom
{
    public class StrikerArrowDisplay : MonoBehaviour
    {
        [Header("Striker")]
        [SerializeField]
        private Striker striker;

        [Header("Scale")]
        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private float scaleUpTo = 3;

        private void Update()
        {
            if (!striker.GetIsDragging())
            {
                lineRenderer.SetPosition(1, Vector2.zero);
                return;
            }
            
            Vector2 power = striker.GetPower();
            lineRenderer.SetPosition(1, power * -scaleUpTo);
        }
    }
}