using UnityEngine;
using CardMatch.Core;

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

        [Header("Game Configuration")]
        [SerializeField] private int rows = 4;
        [SerializeField] private int columns = 4;

        private GameState currentState;
        public GameState CurrentState => currentState;

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
        }

        private void Start()
        {
            InitializeGame();
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
                    // TODO: Generate board, Disable input during initialization
                    break;
                    
                case GameState.Playing:
                    // TODO: Start gameplay, Enable input
                    break;
                    
                case GameState.Paused:
                    // TODO: Pause gameplay, Disable input
                    break;
                    
                case GameState.GameOver:
                    // TODO: Show results, disable input
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
            // TODO: Show game over screen and score display etc
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