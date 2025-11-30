using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using CardMatch.Utils;
using CardMatch;

namespace CardMatch.UI
{
    public partial class HUDController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI matchText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI currentGridSizeText;
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

        [Header("Grid Size Dropdowns")]
        [SerializeField] private TMP_Dropdown rowsDropdown;
        [SerializeField] private TMP_Dropdown columnsDropdown;
        
        // These are now provided by CardMatcherSettings
        private int minGridSize;
        private int maxGridSize;
        private float screenMargin;
        private int timePressureThresholdSeconds;
        private Color normalTimerColor;
        private Color pressureTimerColor;

        private CardMatcherSettings settings;

        [Header("Match Value Display")]
        [SerializeField]
        private TextMeshProUGUI matchRewardText;
        [SerializeField]
        private TextMeshProUGUI mismatchPenaltyText;

        private int moves;
        private int calculatedMaxRows;
        private int calculatedMaxCols;
        private float elapsed;
        private bool timerRunning;
        private int matched;
        private int total;
        private int score;

        private void Start()
        {
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGameClicked);

            // Load settings before initializing dropdown values and runtime UI
            settings = CardMatcherSettings.Get();
            minGridSize = settings.minGridSize;
            maxGridSize = settings.maxGridSize;
            screenMargin = settings.screenMargin;
            timePressureThresholdSeconds = settings.timePressureThresholdSeconds;
            normalTimerColor = settings.normalTimerColor;
            pressureTimerColor = settings.pressureTimerColor;

            // Update in-game display values
            UpdateMatchValuesUI();

            InitializeDropdowns();

            // settings already loaded and applied above

            var r = PlayerPrefsManager.GetRows(settings.defaultRows);
            var c = PlayerPrefsManager.GetColumns(settings.defaultColumns);
            SetDropdownValue(rowsDropdown, r);
            SetDropdownValue(columnsDropdown, c);

            if (rowsDropdown != null) rowsDropdown.onValueChanged.AddListener(OnRowsChanged);
            if (columnsDropdown != null) columnsDropdown.onValueChanged.AddListener(OnColumnsChanged);

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

            // Ensure the display that shows reward/penalty is updated
            UpdateMatchValuesUI();

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
            if (scoreText != null && score > 0)
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

        private void UpdateMatchValuesUI()
        {
            if (settings == null) settings = CardMatcherSettings.Get();
            if (matchRewardText != null) matchRewardText.text = $"Reward: +{settings.matchReward}";
            if (mismatchPenaltyText != null) mismatchPenaltyText.text = $"Penalty: -{settings.mismatchPenalty}";
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

        private void InitializeDropdowns()
        {
            CalculateMaxGridSize();

            var rowOptions = new System.Collections.Generic.List<string>();
            for (int i = minGridSize; i <= calculatedMaxRows; i++)
            {
                rowOptions.Add(i.ToString());
            }

            var colOptions = new System.Collections.Generic.List<string>();
            for (int i = minGridSize; i <= calculatedMaxCols; i++)
            {
                colOptions.Add(i.ToString());
            }

            if (rowsDropdown != null)
            {
                rowsDropdown.ClearOptions();
                rowsDropdown.AddOptions(rowOptions);
            }

            if (columnsDropdown != null)
            {
                columnsDropdown.ClearOptions();
                columnsDropdown.AddOptions(colOptions);
            }

            Debug.Log($"[HUD] Screen-based grid limits: {calculatedMaxRows}x{calculatedMaxCols}");
        }

        private void CalculateMaxGridSize()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                calculatedMaxRows = maxGridSize;
                calculatedMaxCols = maxGridSize;
                Debug.LogWarning("[HUD] Camera.main not found, using default max grid size");
                return;
            }

            var boardManager = GetGameManager()?.BoardManager;
            if (boardManager == null)
            {
                calculatedMaxRows = maxGridSize;
                calculatedMaxCols = maxGridSize;
                Debug.LogWarning("[HUD] BoardManager not found, using default max grid size");
                return;
            }

            // Get actual card size and spacing from BoardManager
            float cardWidth = boardManager.CardSize.x;
            float cardHeight = boardManager.CardSize.y;
            float spacing = boardManager.CardSpacing;

            // Get visible screen space in world units
            float screenHeight = cam.orthographicSize * 2f;
            float screenWidth = screenHeight * cam.aspect;

            // Reserve space for HUD (top and bottom margins)
            float hudReservedHeight = screenHeight * 0.25f;
            float usableHeight = screenHeight - hudReservedHeight;
            float usableWidth = screenWidth * (1f - screenMargin * 2f);

            // Calculate max rows and columns that fit
            calculatedMaxRows = Mathf.FloorToInt((usableHeight + spacing) / (cardHeight + spacing));
            calculatedMaxCols = Mathf.FloorToInt((usableWidth + spacing) / (cardWidth + spacing));

            // Clamp to reasonable limits
            calculatedMaxRows = Mathf.Clamp(calculatedMaxRows, minGridSize, maxGridSize);
            calculatedMaxCols = Mathf.Clamp(calculatedMaxCols, minGridSize, maxGridSize);

            // Ensure even total for pairs
            int maxTotal = calculatedMaxRows * calculatedMaxCols;
            if (maxTotal % 2 != 0)
            {
                if (calculatedMaxCols > minGridSize)
                    calculatedMaxCols--;
                else if (calculatedMaxRows > minGridSize)
                    calculatedMaxRows--;
            }

            Debug.Log($"[HUD] Screen: {screenWidth:F1}x{screenHeight:F1}, Usable: {usableWidth:F1}x{usableHeight:F1}, CardSize: {cardWidth}x{cardHeight}, Max grid: {calculatedMaxRows}x{calculatedMaxCols}");
        }

        private void SetDropdownValue(TMP_Dropdown dropdown, int value)
        {
            if (dropdown == null) return;

            int index = value - minGridSize;
            if (index >= 0 && index < dropdown.options.Count)
            {
                dropdown.value = index;
            }
        }

        private int GetDropdownValue(TMP_Dropdown dropdown, int fallback)
        {
            if (dropdown == null) return fallback;
            
            // Get the actual text value from dropdown
            if (dropdown.value >= 0 && dropdown.value < dropdown.options.Count)
            {
                if (int.TryParse(dropdown.options[dropdown.value].text, out int value))
                {
                    return value;
                }
            }
            
            return fallback;
        }

        private void OnRowsChanged(int index)
        {
            var value = GetDropdownValue(rowsDropdown, 4);
            PlayerPrefsManager.SetRows(value);
            PlayerPrefsManager.Save();
        }

        private void OnColumnsChanged(int index)
        {
            var value = GetDropdownValue(columnsDropdown, 4);
            PlayerPrefsManager.SetColumns(value);
            PlayerPrefsManager.Save();
        }
        
        private Managers.GameManager GetGameManager()
        {
            return Managers.GameManager.Instance;
        }
    }
}
