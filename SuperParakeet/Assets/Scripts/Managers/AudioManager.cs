using UnityEngine;
using CardMatch;

namespace CardMatch.Managers
{
    /// <summary>
    /// Manages all game audio with drag-and-drop clips.
    /// Singleton pattern with object pooling for performance.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private CardMatcherSettings settings;
        private AudioClip flipClip => settings != null ? settings.flipClip : null;
        private AudioClip matchClip => settings != null ? settings.matchClip : null;
        private AudioClip mismatchClip => settings != null ? settings.mismatchClip : null;
        private AudioClip gameOverClip => settings != null ? settings.gameOverClip : null;

        private float masterVolume = 1f;
        private float sfxVolume = 1f;

        // (settings defined above)

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

            // Initialize settings and volumes from centralized settings (if available)
            settings = CardMatcherSettings.Get();
            if (settings != null)
            {
                masterVolume = settings.masterVolume;
                sfxVolume = settings.sfxVolume;
            }
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