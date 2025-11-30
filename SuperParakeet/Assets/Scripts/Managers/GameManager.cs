using UnityEngine;
using CardMatch.Core;
using CardMatch;
using CardMatch.UI;

namespace CardMatch.Managers
{
    /// <summary>
    /// Main game controller using Singleton pattern
    /// Manages game state and coordinates between managers
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Manager References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private HUDController hud;

        [Header("Game Configuration")]
        private int rows;
        private int columns;

        private CardMatcherSettings settings;

        private GameState currentState;
        public GameState CurrentState => currentState;
        public HUDController HUDCanvas => hud;
        public BoardManager BoardManager => boardManager;

        private void Awake()
        {
            // Singleton pattern
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

            // Cache manager references if not assigned
            if (boardManager == null)
                boardManager = GetComponentInChildren<BoardManager>();
            
            if (inputManager == null)
                inputManager = GetComponentInChildren<InputManager>();
            if (hud == null)
            {
                hud = GetComponentInChildren<HUDController>();
                if (hud == null)
                {
                    var prefab = Resources.Load<GameObject>("UI/HUDCanvas");
                    if (prefab != null)
                    {
                        var instance = Instantiate(prefab);
                        hud = instance.GetComponent<HUDController>();
                    }
                }
            }
            // Load settings defaults and player overrides
            settings = CardMatcherSettings.Get();
            rows = CardMatch.Utils.PlayerPrefsManager.GetRows(settings.defaultRows);
            columns = CardMatch.Utils.PlayerPrefsManager.GetColumns(settings.defaultColumns);
        }

        private void Start()
        {
            // Always show setup panel on launch
            if (hud != null)
            {
                hud.OnNewGameClicked();
            }
        }
        /// <summary>
        /// Initialize the game
        /// </summary>
        private void InitializeGame()
        {
            ChangeState(GameState.Initializing);
            
            // Generate the board
            if (boardManager != null)
            {
                boardManager.GenerateBoard(rows, columns);
            }
            
            // Start playing
            ChangeState(GameState.Playing);
            if (hud != null)
            {
                hud.StartTimer();
            }
        }

        /// <summary>
        /// Change game state
        /// </summary>
        public void ChangeState(GameState newState)
        {
            currentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
            
            OnStateChanged(newState);
        }

        /// <summary>
        /// Handle state transitions
        /// </summary>
        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Initializing:
                    if (inputManager != null) inputManager.SetInputEnabled(false);
                    break;
                    
                case GameState.Playing:
                    if (inputManager != null) inputManager.SetInputEnabled(true);
                    if (hud != null) hud.ResumeTimer();
                    break;
                    
                case GameState.Paused:
                    if (inputManager != null) inputManager.SetInputEnabled(false);
                    if (hud != null) hud.PauseTimer();
                    break;
                    
                case GameState.GameOver:
                    if (inputManager != null) inputManager.SetInputEnabled(false);
                    if (hud != null) hud.PauseTimer();
                    OnGameOver();
                    break;
            }
        }

        /// <summary>
        /// Handle game over
        /// </summary>
        private void OnGameOver()
        {
            Debug.Log("[GameManager] Game Over!");
            
            // Play game over sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOver();
            }
            
            if (hud != null) hud.ShowGameOver(rows, columns);
        }
        /// <summary>
        /// Restart the game
        /// </summary>
        public void RestartGame()
        {
            InitializeGame();
        }

        /// <summary>
        /// Start a new game with specific grid size
        /// </summary>
        public void StartNewGame(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            InitializeGame();
        }
    }
}
