using TMPro;
using CardMatch.Core;
using CardMatch.Utils;
using UnityEngine;

namespace CardMatch.UI
{
    public partial class HUDController
    {
        public void ShowGameOver(int rows, int cols)
        {
            HideAllModals();
            DisableGameButtons();
            ShowModalBlocker();

            PlayerPrefsManager.TryUpdateBestScore(rows, cols, score, moves, elapsed);

            var bestData = PlayerPrefsManager.GetBestScoreData(rows, cols);
            var bestScore = PlayerPrefsManager.HasBestScore(rows, cols) ? bestData.score : score;
            var bestMoves = PlayerPrefsManager.HasBestScore(rows, cols) ? bestData.moves : moves;
            var bestTime = PlayerPrefsManager.HasBestScore(rows, cols) ? bestData.time : elapsed;

            currentGridSizeText.text = $"Grid size: {rows} x {cols}";
            UpdateGameOverText(gameOverCurrentText, score, elapsed, moves);
            UpdateGameOverText(gameOverBestText, bestScore, bestTime, bestMoves);

            ShowPanel(gameOverPanel);
        }

        public void HideGameOver()
        {
            HidePanel(gameOverPanel);
            EnableGameButtons();
            HideModalBlocker();
        }

        public void PlayAgain()
        {
            HideGameOver();
            GetGameManager()?.RestartGame();
        }

        public void OpenNewGameFromGameOver()
        {
            HideGameOver();
            OnNewGameClicked();
        }

        public void OnRestartClicked()
        {
            ShowModalBlocker();
            ShowPanel(confirmRestartPanel);
            GetGameManager()?.ChangeState(GameState.Paused);
        }

        public void ConfirmRestart()
        {
            HidePanel(confirmRestartPanel);
            HideModalBlocker();
            GetGameManager()?.RestartGame();
        }

        public void CancelRestart()
        {
            HidePanel(confirmRestartPanel);
            HideModalBlocker();
            GetGameManager()?.ChangeState(GameState.Playing);
        }

        public void OnNewGameClicked()
        {
            ShowModalBlocker();
            ShowPanel(setupPanel);
            GetGameManager()?.ChangeState(GameState.Paused);
        }

        public void ApplyNewGame()
        {
            var settings = CardMatcherSettings.Get();
            var rows = GetDropdownValue(rowsDropdown, settings.defaultRows);
            var cols = GetDropdownValue(columnsDropdown, settings.defaultColumns);

            // Ensure even number of cards (should already be valid from dropdown options)
            var totalCards = rows * cols;
            if (totalCards % 2 != 0)
            {
                Debug.LogWarning($"[HUD] Grid {rows}x{cols} has odd total. Adjusting columns.");
                cols = cols > minGridSize ? cols - 1 : cols + 1;
                SetDropdownValue(columnsDropdown, cols);
            }

            PlayerPrefsManager.SetGridSize(rows, cols);
            
            HidePanel(setupPanel);
            HideModalBlocker();
            
            GetGameManager()?.StartNewGame(rows, cols);
        }
        private void UpdateGameOverText(TextMeshProUGUI textField, int score, float time, int moves)        
        {
            if (textField != null)
            {
                textField.text = $"{score} ({FormatTime(time)} / {moves} moves)";
            }
        }

        private void ShowPanel(GameObject panel)
        {
            if (panel == null) return;
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
        }

        private void HidePanel(GameObject panel)
        {
            if (panel != null) panel.SetActive(false);
        }

        private void ShowModalBlocker()
        {
            ShowPanel(modalBlocker);
        }

        private void HideModalBlocker()
        {
            HidePanel(modalBlocker);
        }

        private void HideAllModals()
        {
            HidePanel(confirmRestartPanel);
            HidePanel(setupPanel);
        }

        private void DisableGameButtons()
        {
            if (restartButton != null) restartButton.interactable = false;
            if (newGameButton != null) newGameButton.interactable = false;
        }

        private void EnableGameButtons()
        {
            if (restartButton != null) restartButton.interactable = true;
            if (newGameButton != null) newGameButton.interactable = true;
        }
    }
}
