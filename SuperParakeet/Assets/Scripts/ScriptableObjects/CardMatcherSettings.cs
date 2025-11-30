using UnityEngine;

namespace CardMatch
{
    [CreateAssetMenu(fileName = "CardMatcherSettings", menuName = "CardMatch/Settings/CardMatcherSettings")]
    public class CardMatcherSettings : ScriptableObject
    {
        [Header("Defaults")]
        public int defaultRows = 4;
        public int defaultColumns = 4;

        [Header("Grid")]
        public int minGridSize = 2;
        public int maxGridSize = 8;
        public float screenMargin = 0.1f;

        [Header("HUD")]
        public int timePressureThresholdSeconds = 60;
        public Color normalTimerColor = Color.white;
        public Color pressureTimerColor = Color.red;

        [Header("Board")]
        public Vector2 cardSize = new Vector2(1f, 1.4f);
        public float cardSpacing = 0.2f;
        public float mismatchDelay = 0.6f;
        public int matchReward = 100;
        public int mismatchPenalty = 5;

        [Header("Audio")]
        public AudioClip flipClip;
        public AudioClip matchClip;
        public AudioClip mismatchClip;
        public AudioClip gameOverClip;
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        private static CardMatcherSettings _instance;

        /// <summary>
        /// Attempts to load the settings asset from Resources if available.
        /// </summary>
        public static CardMatcherSettings Get()
        {
            if (_instance != null) return _instance;

            _instance = Resources.Load<CardMatcherSettings>("CardMatcherSettings");
            if (_instance == null)
            {
                Debug.LogWarning("CardMatcherSettings asset not found in Resources. Using defaults.");
                _instance = CreateInstance<CardMatcherSettings>();
            }

            return _instance;
        }
    }
}
