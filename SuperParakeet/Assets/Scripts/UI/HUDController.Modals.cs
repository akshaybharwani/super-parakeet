using UnityEngine;
using TMPro;
using CardMatch.Core;
using CardMatch.Utils;

namespace CardMatch.UI
{
    public partial class HUDController
    {
        public void ShowGameOver(int rows, int cols)
        {
            if (confirmRestartPanel != null) confirmRestartPanel.SetActive(false);
            if (setupPanel != null) setupPanel.SetActive(false);
            if (restartButton != null) restartButton.interactable = false;
            if (newGameButton != null) newGameButton.interactable = false;
            if (modalBlocker != null)
            {
                modalBlocker.SetActive(true);
                modalBlocker.transform.SetAsLastSibling();
            }

            // Get current best scores
            var bestMoves = PlayerPrefsManager.GetBestMoves(rows, cols, moves);
            var bestTime = PlayerPrefsManager.GetBestTime(rows, cols, elapsed);

            // Update if current is better
            if (!PlayerPrefsManager.HasBestMoves(rows, cols) || moves < bestMoves)
            {
                bestMoves = moves;
                PlayerPrefsManager.SetBestMoves(rows, cols, bestMoves);
            }

            if (!PlayerPrefsManager.HasBestTime(rows, cols) || elapsed < bestTime)
            {
                bestTime = elapsed;
                PlayerPrefsManager.SetBestTime(rows, cols, bestTime);
            }

            PlayerPrefsManager.Save();
            if (gameOverCurrentText != null)
            {
                gameOverCurrentText.text = FormatTime(elapsed) + " / " + moves.ToString();
            }
            if (gameOverBestText != null)
            {
                gameOverBestText.text = FormatTime(bestTime) + " / " + bestMoves.ToString();
            }
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel.transform.SetAsLastSibling();
            }
        }

        public void HideGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (restartButton != null) restartButton.interactable = true;
            if (newGameButton != null) newGameButton.interactable = true;
            if (modalBlocker != null) modalBlocker.SetActive(false);
        }

        public void PlayAgain()
        {
            HideGameOver();
            Managers.GameManager.Instance.RestartGame();
        }

        public void OpenNewGameFromGameOver()
        {
            HideGameOver();
            OnNewGameClicked();
        }

        public void OnRestartClicked()
        {
            if (modalBlocker != null)
            {
                modalBlocker.SetActive(true);
                modalBlocker.transform.SetAsLastSibling();
            }
            if (confirmRestartPanel != null)
            {
                confirmRestartPanel.SetActive(true);
                confirmRestartPanel.transform.SetAsLastSibling();
            }
            Managers.GameManager.Instance.ChangeState(GameState.Paused);
        }

        public void ConfirmRestart()
        {
            if (confirmRestartPanel != null) confirmRestartPanel.SetActive(false);
            if (modalBlocker != null) modalBlocker.SetActive(false);
            Managers.GameManager.Instance.RestartGame();
        }

        public void CancelRestart()
        {
            if (confirmRestartPanel != null) confirmRestartPanel.SetActive(false);
            if (modalBlocker != null) modalBlocker.SetActive(false);
            Managers.GameManager.Instance.ChangeState(GameState.Playing);
        }

        public void OnNewGameClicked()
        {
            if (modalBlocker != null)
            {
                modalBlocker.SetActive(true);
                modalBlocker.transform.SetAsLastSibling();
            }
            if (setupPanel != null)
            {
                setupPanel.SetActive(true);
                setupPanel.transform.SetAsLastSibling();
            }
            Managers.GameManager.Instance.ChangeState(GameState.Paused);
        }

        public void ApplyNewGame()
        {
            var rows = ParseInput(rowsInput, 4);
            var cols = ParseInput(columnsInput, 4);
            PlayerPrefsManager.SetGridSize(rows, cols);
            if (setupPanel != null) setupPanel.SetActive(false);
            if (modalBlocker != null) modalBlocker.SetActive(false);
            Managers.GameManager.Instance.StartNewGame(rows, cols);
        }

        private void OnRowsEdited(string text)
        {
            var v = ParseInput(rowsInput, 4);
            PlayerPrefsManager.SetRows(v);
            PlayerPrefsManager.Save();
        }

        private void OnColumnsEdited(string text)
        {
            var v = ParseInput(columnsInput, 4);
            PlayerPrefsManager.SetColumns(v);
            PlayerPrefsManager.Save();
        }

        private int ParseInput(TMP_InputField field, int fallback)
        {
            if (field == null) return fallback;
            int parsed;
            if (int.TryParse(field.text, out parsed)) return parsed;
            return fallback;
        }
    }
}
