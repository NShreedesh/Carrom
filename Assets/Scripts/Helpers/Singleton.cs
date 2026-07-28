using UnityEngine;

namespace Helpers
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        [SerializeField]
        private bool dontDestroyOnLoad;

        public virtual void Awake()
        {
            if (Instance == null)
            {
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(this);
                Instance = FindAnyObjectByType<T>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
