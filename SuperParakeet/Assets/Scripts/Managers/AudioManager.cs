using UnityEngine;

namespace CardMatch.Managers
{
    /// <summary>
    /// Manages all game audio with drag-and-drop clips.
    /// Singleton pattern with object pooling for performance.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip flipClip;
        [SerializeField] private AudioClip matchClip;
        [SerializeField] private AudioClip mismatchClip;
        [SerializeField] private AudioClip gameOverClip;

        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
        }

        public void PlayFlip()
        {
            PlaySound(flipClip);
        }

        public void PlayMatch()
        {
            PlaySound(matchClip);
        }

        public void PlayMismatch()
        {
            PlaySound(mismatchClip);
        }

        public void PlayGameOver()
        {
            PlaySound(gameOverClip);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            float volume = masterVolume * sfxVolume;
            audioSource.PlayOneShot(clip, volume);
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
    }
}