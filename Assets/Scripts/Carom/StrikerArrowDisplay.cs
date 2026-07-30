using UnityEngine;

namespace Scripts.Carom
{
    public class StrikerArrowDisplay : MonoBehaviour
    {
        [Header("Scale")]
        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private float scaleUpTo = 3;

        public void UpdateStrikerArrowDisplay(Vector2 power)
        {
            lineRenderer.SetPosition(1, power * -scaleUpTo);
        }

        public void Reset()
        {

            lineRenderer.SetPosition(1, Vector2.zero);
        }
    }
}