using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardMatch.Core;
using CardMatch.Data;
using CardMatch.Utils;
using CardMatch.UI;
using CardMatch;

namespace CardMatch.Managers
{
    /// <summary>
    /// Manages the game board, card generation, and layout.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Card cardPrefab;

        [Header("Board Configuration")]
        [SerializeField] private Transform boardContainer;
        // card size/spacing moved to centralized settings
        public Vector2 CardSize => settings.cardSize;
        public float CardSpacing => settings.cardSpacing;

        private CardMatcherSettings settings;

        private void Awake()
        {
            settings = CardMatcherSettings.Get();
        }

        // Match configuration is now derived from settings
        // match values moved to centralized settings
        public float MismatchDelay => settings.mismatchDelay;
        public int MatchReward => settings.matchReward;
        public int MismatchPenalty => settings.mismatchPenalty;

        private readonly List<Card> spawnedCards = new();
        private readonly List<Card> selection = new();
        private readonly Queue<Card> clickQueue = new();

        private int rows;
        private int columns;
        private bool isProcessingSelection;
        private int matchedPairs;
        private int totalPairs;
        private int score;

        /// <summary>
        /// Generate a new game board.
        /// </summary>
        public void GenerateBoard(int rows, int columns)
        {
            ResetInteraction();
            ClearBoard();

            this.rows = rows;
            this.columns = columns;

            var totalCards = this.rows * this.columns;

            // Validate even number of cards
            if (totalCards % 2 != 0)
            {
                Debug.LogError("[BoardManager] Total cards must be even for matching pairs!");
                return;
            }

            Debug.Log($"[BoardManager] Generating {this.rows}x{this.columns} board ({totalCards} cards)");

            // Create card data pairs
            var cardDeck = CreateCardDeck(totalCards / 2);

            totalPairs = totalCards / 2;
            matchedPairs = 0;
            score = 0;

            var hud = GetHud();
            if (hud != null)
            {
                hud.ResetHUD(totalPairs);
                hud.SetScore(score);
            }

            // Shuffle the deck
            ShuffleDeck(cardDeck);

            // Spawn cards
            SpawnCards(cardDeck);
        }

        /// <summary>
        /// Create a deck of card pairs.
        /// </summary>
        private List<CardData> CreateCardDeck(int pairCount)
        {
            var deck = new List<CardData>();

            // Create card data with IDs
            for (var i = 0; i < pairCount; i++)
            {
                var data = CardData.Create(i);

                // Add pair
                deck.Add(data);
                deck.Add(data);
            }

            return deck;
        }

        /// <summary>
        /// Shuffle card deck using Fisher-Yates algorithm.
        /// </summary>
        private void ShuffleDeck(List<CardData> deck)
        {
            // Ensure random seed is fresh
            Random.InitState((int)System.DateTime.Now.Ticks);

            for (var i = deck.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                var temp = deck[i];
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }

            // Debug: Log first few IDs to verify shuffle
            Debug.Log($"[BoardManager] Shuffled deck preview: {string.Join(", ", deck.GetRange(0, Mathf.Min(8, deck.Count)).ConvertAll(c => c.Id))}");
        }

        /// <summary>
        /// Spawn cards on the board with proper layout.
        /// </summary>
        private void SpawnCards(List<CardData> cardDeck)
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[BoardManager] Card prefab is not assigned!");
                return;
            }

            // Calculate grid center offset
            var totalWidth = (columns * CardSize.x) + ((columns - 1) * CardSpacing);
            var totalHeight = (rows * CardSize.y) + ((rows - 1) * CardSpacing);
            var startPosition = new Vector3(-totalWidth / 2f, totalHeight / 2f, 0f);

            SpawnCards(cardDeck, startPosition);
        }

        /// <summary>
        /// Spawn cards on the board using calculated positions and initialize them with relevant data.
        /// Manages event subscriptions for card interactions.
        /// </summary>
        private void SpawnCards(List<CardData> cardDeck, Vector3 startPosition)
        {
            var cardIndex = 0;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var position = startPosition + new Vector3(
                        col * (CardSize.x + CardSpacing),
                        -row * (CardSize.y + CardSpacing),
                        0f);

                    var card = Instantiate(cardPrefab, boardContainer);

                    // Get symbol for this card
                    var symbol = CardSymbolProvider.GetSymbol(cardDeck[cardIndex].Id);

                    // Initialize card with all data
                    card.Initialize(cardDeck[cardIndex], col, row, position, symbol);

                    // Subscribe to card click event
                    card.OnCardClicked += OnCardClicked;

                    spawnedCards.Add(card);
                    cardIndex++;
                }
            }
        }

        /// <summary>
        /// Handle card click event.
        /// </summary>
        private void OnCardClicked(Card card)
        {
            // Debug.Log($"[BoardManager] Card clicked: ID={card.Id}, Position=({card.GridX},{card.GridY})");

            if (card.State != CardState.Hidden)
            {
                return;
            }

            if (selection.Count < 2)
            {
                card.Reveal();
                selection.Add(card);

                if (selection.Count == 2 && !isProcessingSelection)
                {
                    isProcessingSelection = true;
                    StartCoroutine(ProcessSelection());
                }
            }
            else if (!clickQueue.Contains(card))
            {
                clickQueue.Enqueue(card);
            }
        }

        /// <summary>
        /// Process the current card selection for a potential match.
        /// </summary>
        private IEnumerator ProcessSelection()
        {
            yield return new WaitUntil(() => selection.TrueForAll(c => c.State == CardState.Revealed));

            var hud = GetHud();
            if (hud != null)
            {
                hud.IncrementMoves();
            }

            var a = selection[0];
            var b = selection[1];

            if (a == null || b == null)
            {
                selection.Clear();
                isProcessingSelection = false;
                yield break;
            }

            if (a.Id == b.Id)
            {
                a.Match();
                b.Match();

                matchedPairs++;
                score += MatchReward;

                // Play match sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMatch();
                }

                if (hud != null)
                {
                    hud.SetMatchProgress(matchedPairs, totalPairs);
                    hud.PlayMilestone();
                    hud.SetScore(score);
                }

                if (matchedPairs >= totalPairs && GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.GameOver);
                }
            }
            else
            {
                // Play mismatch sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMismatch();
                }

                yield return new WaitForSeconds(MismatchDelay);

                if (a != null)
                {
                    a.Hide();
                }

                if (b != null)
                {
                    b.Hide();
                }

                yield return new WaitUntil(() => a.State == CardState.Hidden && b.State == CardState.Hidden);

                score = Mathf.Max(0, score - MismatchPenalty);

                if (hud != null)
                {
                    hud.SetScore(score);
                }
            }

            selection.Clear();

            while (selection.Count < 2 && clickQueue.Count > 0)
            {
                var next = clickQueue.Dequeue();
                if (next != null && next.State == CardState.Hidden)
                {
                    next.Reveal();
                    selection.Add(next);
                }
            }

            if (selection.Count == 2)
            {
                StartCoroutine(ProcessSelection());
            }
            else
            {
                isProcessingSelection = false;
            }
        }

        /// <summary>
        /// Clear all cards from the board.
        /// </summary>
        private void ClearBoard()
        {
            ResetInteraction();

            foreach (var card in spawnedCards)
            {
                if (card == null)
                {
                    continue;
                }

                card.OnCardClicked -= OnCardClicked;
                Destroy(card.gameObject);
            }

            spawnedCards.Clear();
        }

        /// <summary>
        /// Get all cards on the board.
        /// </summary>
        public List<Card> GetAllCards()
        {
            return new(spawnedCards);
        }

        private void OnDestroy()
        {
            // Cleanup event subscriptions
            foreach (var card in spawnedCards)
            {
                if (card == null)
                {
                    continue;
                }

                card.OnCardClicked -= OnCardClicked;
            }
        }

        /// <summary>
        /// Reset interaction state and clear current selections/queue.
        /// </summary>
        private void ResetInteraction()
        {
            StopAllCoroutines();
            isProcessingSelection = false;
            selection.Clear();
            clickQueue.Clear();
        }

        private HUDController GetHud()
        {
            return GameManager.Instance != null ? GameManager.Instance.HUDCanvas : null;
        }
    }
}
