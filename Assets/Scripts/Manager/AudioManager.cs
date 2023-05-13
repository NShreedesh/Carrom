using UnityEngine;

namespace Scripts.Manager
{
    public class AudioManager : MonoBehaviour
    {
        [field: Header("Singleton")]
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio")]
        [SerializeField]
        private AudioSource soundFXAudioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySoundFx(AudioClip clip, float volume = 1)
        {
            soundFXAudioSource.PlayOneShot(clip, volume);
        }
    }
}