using UnityEngine;

namespace TrafficJam.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }


        [Header("Gameplay SFX")]
        [SerializeField]
        AudioClip moneyPickup;

        [SerializeField]
        AudioClip crash;

        [SerializeField]
        AudioSource audioSource;

        private void Awake()
        {
            // Ensure only one AudioManager exists.
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Plays the money pickup sound effect.
        /// </summary>
        /// <param name="volume"></param>
        public void PlayMoneyPickup(float volume)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(moneyPickup);
        }

        /// <summary>
        /// Plays the traffic collision sound effect.
        /// </summary>
        /// <param name="volume"></param>
        public void PlayCrash(float volume)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(crash);
        }
    }
}
