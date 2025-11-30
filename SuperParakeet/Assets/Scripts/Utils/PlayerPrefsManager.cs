using UnityEngine;

namespace CardMatch.Utils
{
    /// <summary>
    /// Centralized PlayerPrefs operations with type-safe access and no magic strings.
    /// </summary>
    public static class PlayerPrefsManager
    {
        private const string RowsKey = "cm_rows";
        private const string ColumnsKey = "cm_cols";
        private const string BestScoreKeyFormat = "cm_best_score_{0}x{1}";
        private const string BestMovesKeyFormat = "cm_best_moves_{0}x{1}";
        private const string BestTimeKeyFormat = "cm_best_time_{0}x{1}";

        public static int GetRows(int defaultValue = 4)
        {
            return PlayerPrefs.GetInt(RowsKey, defaultValue);
        }

        public static void SetRows(int rows)
        {
            PlayerPrefs.SetInt(RowsKey, rows);
        }

        public static int GetColumns(int defaultValue = 4)
        {
            return PlayerPrefs.GetInt(ColumnsKey, defaultValue);
        }

        public static void SetColumns(int columns)
        {
            PlayerPrefs.SetInt(ColumnsKey, columns);
        }

        public static void SetGridSize(int rows, int columns)
        {
            SetRows(rows);
            SetColumns(columns);
            Save();
        }

        public static bool HasBestScore(int rows, int columns)
        {
            string key = string.Format(BestScoreKeyFormat, rows, columns);
            return PlayerPrefs.HasKey(key);
        }

        public static int GetBestScore(int rows, int columns, int defaultValue = 0)
        {
            string key = string.Format(BestScoreKeyFormat, rows, columns);
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static void SetBestScore(int rows, int columns, int score)
        {
            string key = string.Format(BestScoreKeyFormat, rows, columns);
            PlayerPrefs.SetInt(key, score);
        }

        public static bool HasBestMoves(int rows, int columns)
        {
            string key = string.Format(BestMovesKeyFormat, rows, columns);
            return PlayerPrefs.HasKey(key);
        }

        public static int GetBestMoves(int rows, int columns, int defaultValue = int.MaxValue)
        {
            string key = string.Format(BestMovesKeyFormat, rows, columns);
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static void SetBestMoves(int rows, int columns, int moves)
        {
            string key = string.Format(BestMovesKeyFormat, rows, columns);
            PlayerPrefs.SetInt(key, moves);
        }

        public static bool HasBestTime(int rows, int columns)
        {
            string key = string.Format(BestTimeKeyFormat, rows, columns);
            return PlayerPrefs.HasKey(key);
        }

        public static float GetBestTime(int rows, int columns, float defaultValue = float.MaxValue)
        {
            string key = string.Format(BestTimeKeyFormat, rows, columns);
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static void SetBestTime(int rows, int columns, float time)
        {
            string key = string.Format(BestTimeKeyFormat, rows, columns);
            PlayerPrefs.SetFloat(key, time);
        }

        public static bool TryUpdateBestScore(int rows, int columns, int currentScore, int currentMoves, float currentTime)
        {
            bool isNewRecord = false;

            if (!HasBestScore(rows, columns) || currentScore > GetBestScore(rows, columns))
            {
                SetBestScore(rows, columns, currentScore);
                SetBestMoves(rows, columns, currentMoves);
                SetBestTime(rows, columns, currentTime);
                isNewRecord = true;
                Save();
            }

            return isNewRecord;
        }

        public static (int score, int moves, float time) GetBestScoreData(int rows, int columns)
        {
            int score = GetBestScore(rows, columns);
            int moves = GetBestMoves(rows, columns);
            float time = GetBestTime(rows, columns);
            return (score, moves, time);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            Save();
        }

        public static void ClearBestScores(int rows, int columns)
        {
            string scoreKey = string.Format(BestScoreKeyFormat, rows, columns);
            string movesKey = string.Format(BestMovesKeyFormat, rows, columns);
            string timeKey = string.Format(BestTimeKeyFormat, rows, columns);

            if (PlayerPrefs.HasKey(scoreKey))
                PlayerPrefs.DeleteKey(scoreKey);

            if (PlayerPrefs.HasKey(movesKey))
                PlayerPrefs.DeleteKey(movesKey);

            if (PlayerPrefs.HasKey(timeKey))
                PlayerPrefs.DeleteKey(timeKey);

            Save();
        }
    }
}