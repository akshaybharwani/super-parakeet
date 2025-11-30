using UnityEditor;
using UnityEngine;
using CardMatch;

namespace CardMatch.Editor
{
    public class CardMatcherSettingsEditorWindow : EditorWindow
    {
        private CardMatcherSettings settings;
        private const string ResourcePath = "Assets/Resources/CardMatcherSettings.asset";

        [MenuItem("Tools/Card Matcher/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<CardMatcherSettingsEditorWindow>("Card Matcher Settings");
            window.minSize = new Vector2(420, 500);
        }

        private void OnEnable()
        {
            settings = Resources.Load<CardMatcherSettings>("CardMatcherSettings");

            if (settings == null)
            {
                // Do not automatically create the asset - show a button instead
                settings = CardMatcherSettings.Get();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Card Matcher Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (settings == null || AssetDatabase.GetAssetPath(settings) == string.Empty)
            {
                EditorGUILayout.HelpBox("No CardMatcherSettings asset found in Resources. Create a new one to persist settings.", MessageType.Warning);
                if (GUILayout.Button("Create CardMatcherSettings asset in Resources"))
                {
                    CreateAsset();
                }

                // Draw readonly runtime defaults
                DrawSettingsUI(settings, true);
                return;
            }

            // Editable fields for the ScriptableObject asset
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            DrawSettingsUI(settings, false);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Ping Settings in Project"))
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
        }

        private void DrawSettingsUI(CardMatcherSettings s, bool readOnly)
        {
            if (s == null)
            {
                EditorGUILayout.LabelField("No runtime settings available", EditorStyles.miniLabel);
                return;
            }

            using (new EditorGUI.DisabledScope(readOnly))
            {
                EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
                s.defaultRows = EditorGUILayout.IntField("Default Rows", s.defaultRows);
                s.defaultColumns = EditorGUILayout.IntField("Default Columns", s.defaultColumns);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
                s.minGridSize = EditorGUILayout.IntField("Min Grid Size", s.minGridSize);
                s.maxGridSize = EditorGUILayout.IntField("Max Grid Size", s.maxGridSize);
                s.screenMargin = EditorGUILayout.Slider("Screen Margin", s.screenMargin, 0f, 0.5f);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("HUD", EditorStyles.boldLabel);
                s.timePressureThresholdSeconds = EditorGUILayout.IntField("Time Pressure Secs", s.timePressureThresholdSeconds);
                s.normalTimerColor = EditorGUILayout.ColorField("Normal Timer Color", s.normalTimerColor);
                s.pressureTimerColor = EditorGUILayout.ColorField("Pressure Timer Color", s.pressureTimerColor);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Board", EditorStyles.boldLabel);
                s.cardSize = EditorGUILayout.Vector2Field("Card Size", s.cardSize);
                s.cardSpacing = EditorGUILayout.FloatField("Card Spacing", s.cardSpacing);
                s.mismatchDelay = EditorGUILayout.FloatField("Mismatch Delay", s.mismatchDelay);
                s.matchReward = EditorGUILayout.IntField("Match Reward", s.matchReward);
                s.mismatchPenalty = EditorGUILayout.IntField("Mismatch Penalty", s.mismatchPenalty);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
                s.flipClip = (AudioClip)EditorGUILayout.ObjectField("Flip Clip", s.flipClip, typeof(AudioClip), false);
                s.matchClip = (AudioClip)EditorGUILayout.ObjectField("Match Clip", s.matchClip, typeof(AudioClip), false);
                s.mismatchClip = (AudioClip)EditorGUILayout.ObjectField("Mismatch Clip", s.mismatchClip, typeof(AudioClip), false);
                s.gameOverClip = (AudioClip)EditorGUILayout.ObjectField("GameOver Clip", s.gameOverClip, typeof(AudioClip), false);
                s.masterVolume = EditorGUILayout.Slider("Master Volume", s.masterVolume, 0f, 1f);
                s.sfxVolume = EditorGUILayout.Slider("SFX Volume", s.sfxVolume, 0f, 1f);
            }
        }

        private void CreateAsset()
        {
            var dir = System.IO.Path.GetDirectoryName(ResourcePath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            settings = CreateInstance<CardMatcherSettings>();
            AssetDatabase.CreateAsset(settings, ResourcePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = settings;

            Debug.Log("Created CardMatcherSettings asset at Resources/CardMatcherSettings.asset");
        }
    }
}
