using UnityEngine;

namespace Helpers
{
    public class FrameRateManager : MonoBehaviour
    {
        [SerializeField]
        private bool shouldSetGivenFps;
        [SerializeField]
        private int fps = 60;

        private void Start()
        {
            if (shouldSetGivenFps)
            {
                Application.targetFrameRate = fps;
                QualitySettings.vSyncCount = 0;
            }
            else
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 1;
            }
        }
    }
}