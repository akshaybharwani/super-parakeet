using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CardMatch.Utils;

namespace CardMatch.UI
{
    public partial class HUDController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI matchText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI gameOverCurrentText;
        [SerializeField] private TextMeshProUGUI gameOverBestText;
        [SerializeField] private Image matchProgress;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button newGameButton;

        [Header("Panels")]
        [SerializeField] private GameObject confirmRestartPanel;
        [SerializeField] private GameObject setupPanel;
        [SerializeField] private GameObject modalBlocker;
        [SerializeField] private GameObject gameOverPanel;

        [Header("InputFields")]
        [SerializeField] private TMP_InputField rowsInput;
        [SerializeField] private TMP_InputField columnsInput;
        
        [Header("Timer Settings")]
        [SerializeField] private int timePressureThresholdSeconds = 60;
        [SerializeField] private Color normalTimerColor = Color.white;
        [SerializeField] private Color pressureTimerColor = Color.red;

        private int moves;
        private float elapsed;
        private bool timerRunning;
        private int matched;
        private int total;
        private int score;

        private void Awake()
        {
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClicked);

            if (rowsInput != null) rowsInput.onEndEdit.AddListener(OnRowsEdited);
            if (columnsInput != null) columnsInput.onEndEdit.AddListener(OnColumnsEdited);

            var r = PlayerPrefsManager.GetRows(4);
            var c = PlayerPrefsManager.GetColumns(4);
            if (rowsInput != null) rowsInput.text = r.ToString();
            if (columnsInput != null) columnsInput.text = c.ToString();

            moves = 0;
            elapsed = 0f;
            matched = 0;
            total = 0;
            score = 0;
            timerRunning = false;

            UpdateMovesText();
            UpdateTimerText();
            UpdateMatchText();
            UpdateScoreText();

            if (matchProgress != null)
            {
                matchProgress.type = Image.Type.Filled;
                matchProgress.fillMethod = Image.FillMethod.Horizontal;
                matchProgress.fillOrigin = (int)Image.OriginHorizontal.Left;
                matchProgress.fillAmount = 0f;
            }

            if (confirmRestartPanel != null) confirmRestartPanel.SetActive(false);
            if (setupPanel != null) setupPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (modalBlocker != null) modalBlocker.SetActive(false);
        }

        private void Update()
        {
            if (!timerRunning) return;

            elapsed += Time.deltaTime;
            UpdateTimerText();
            UpdateTimerColor();
        }

        public void ResetHUD(int totalPairs)
        {
            moves = 0;
            elapsed = 0f;
            matched = 0;
            total = totalPairs;
            score = 0;
            timerRunning = false;

            UpdateMovesText();
            UpdateTimerText();
            UpdateMatchText();
            UpdateScoreText();

            if (matchProgress != null)
            {
                matchProgress.fillAmount = 0f;
            }

            if (restartButton != null) restartButton.interactable = true;
            if (newGameButton != null) newGameButton.interactable = true;

            if (timerText != null) timerText.color = normalTimerColor;
        }

        public void StartTimer()
        {
            timerRunning = true;
        }

        public void PauseTimer()
        {
            timerRunning = false;
        }

        public void ResumeTimer()
        {
            timerRunning = true;
        }

        public void IncrementMoves()
        {
            moves++;
            UpdateMovesText();
            if (movesText != null)
            {
                movesText.transform.DOKill();
                movesText.transform.localScale = Vector3.one;
                movesText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.2f, 1, 0.5f);
            }
        }

        public void SetMatchProgress(int currentMatched, int totalPairs)
        {
            matched = currentMatched;
            total = totalPairs;

            UpdateMatchText();

            if (matchProgress != null)
            {
                // Ensure the bar fills in front of the background but behind text
                matchProgress.fillAmount = total > 0 ? (float)matched / total : 0f;
            }
        }

        public void PlayMilestone()
        {
            if (matchText != null)
            {
                matchText.transform.DOKill();
                matchText.transform.localScale = Vector3.one;
                matchText.transform
                    .DOScale(1.2f, 0.15f)
                    .SetLoops(2, LoopType.Yoyo);
            }
        }

        public void SetScore(int value)
        {
            score = value < 0 ? 0 : value;
            UpdateScoreText();
            if (scoreText != null)
            {
                scoreText.transform.DOKill();
                scoreText.transform.localScale = Vector3.one;
                scoreText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.2f, 1, 0.5f);
            }
        }

        public void AddScore(int delta)
        {
            SetScore(score + delta);
        }

        private void UpdateMovesText()
        {
            if (movesText != null) movesText.text = moves.ToString();
        }

        private void UpdateTimerText()
        {
            var m = Mathf.FloorToInt(elapsed / 60f);
            var s = Mathf.FloorToInt(elapsed % 60f);
            if (timerText != null) timerText.text = m.ToString("00") + ":" + s.ToString("00");
        }

        private void UpdateTimerColor()
        {
            if (timerText == null) return;

            timerText.color = elapsed >= timePressureThresholdSeconds
                ? pressureTimerColor
                : normalTimerColor;
        }

        private void UpdateMatchText()
        {
            if (matchText != null) matchText.text = matched + "/" + total;
        }

        private void UpdateScoreText()
        {
            if (scoreText != null) scoreText.text = score.ToString();
        }

        private string FormatTime(float t)
        {
            var m = Mathf.FloorToInt(t / 60f);
            var s = Mathf.FloorToInt(t % 60f);
            return m.ToString("00") + ":" + s.ToString("00");
        }
    }
}
