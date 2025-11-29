using System.Collections.Generic;
using UnityEngine;
using CardMatch.Core;
using CardMatch.Data;
using CardMatch.Utils;

namespace CardMatch.Managers
{
    /// <summary>
    /// Manages the game board, card generation, and layout
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Card cardPrefab;

        [Header("Board Configuration")]
        [SerializeField] private Transform boardContainer;
        [SerializeField] private Vector2 cardSize = new(1f, 1.4f);
        [SerializeField] private float cardSpacing = 0.2f;

        private List<Card> spawnedCards = new();
        private int rows;
        private int columns;        
        private readonly List<Card> selection = new();
        private readonly Queue<Card> clickQueue = new();
        [SerializeField] private float mismatchDelay = 0.6f;
        private bool isProcessingSelection = false;
        
        /// <summary>
        /// Generate a new game board
        /// </summary>
        public void GenerateBoard(int rows, int columns)
        {
            ClearBoard();

            this.rows = rows;
            this.columns = columns;
            int totalCards = this.rows * this.columns;

            // Validate even number of cards
            if (totalCards % 2 != 0)
            {
                Debug.LogError("[BoardManager] Total cards must be even for matching pairs!");
                return;
            }

            Debug.Log($"[BoardManager] Generating {this.rows}x{this.columns} board ({totalCards} cards)");

            // Create card data pairs
            var cardDeck = CreateCardDeck(totalCards / 2);

            // Shuffle the deck
            ShuffleDeck(cardDeck);

            // Spawn cards
            SpawnCards(cardDeck);
        }

        /// <summary>
        /// Create a deck of card pairs
        /// </summary>
        private List<CardData> CreateCardDeck(int pairCount)
        {
            List<CardData> deck = new List<CardData>();

            // Create card data with IDs
            for (int i = 0; i < pairCount; i++)
            {
                CardData data = CardData.Create(i);

                // Add pair
                deck.Add(data);
                deck.Add(data);
            }

            return deck;
        }
        /// <summary>
        /// Shuffle card deck using Fisher-Yates algorithm
        /// </summary>
        private void ShuffleDeck(List<CardData> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                var temp = deck[i];
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
        }

        /// <summary>
        /// Spawn cards on the board with proper layout
        /// </summary>
        private void SpawnCards(List<CardData> cardDeck)
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[BoardManager] Card prefab is not assigned!");
                return;
            }

            // Calculate grid center offset
            float totalWidth = (columns * cardSize.x) + ((columns - 1) * cardSpacing);
            float totalHeight = (rows * cardSize.y) + ((rows - 1) * cardSpacing);
            Vector3 startPosition = new(-totalWidth / 2f, totalHeight / 2f, 0);
            SpawnCards(cardDeck, startPosition);
        }

        /// <summary>
        /// Spawns cards on the board using calculated positions and initializes them with relevant data.
        /// Manages event subscriptions for card interactions.
        /// </summary>
        private void SpawnCards(List<CardData> cardDeck, Vector3 startPosition)
        {
            var cardIndex = 0;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    Vector3 position = startPosition + new Vector3(
                        col * (cardSize.x + cardSpacing),
                        -row * (cardSize.y + cardSpacing),
                        0
                    );

                    var card = Instantiate(cardPrefab, boardContainer);

                    // Get symbol for this card
                    string symbol = CardSymbolProvider.GetSymbol(cardDeck[cardIndex].Id);

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
        /// Handle card click event
        /// </summary>
        private void OnCardClicked(Card card)
        {
            Debug.Log($"[BoardManager] Card clicked: ID={card.Id}, Position=({card.GridX},{card.GridY})");
            if (card.State != CardState.Hidden)
                return;

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
            else
            {
                if (!clickQueue.Contains(card))
                {
                    clickQueue.Enqueue(card);
                }
            }
        }

        private System.Collections.IEnumerator ProcessSelection()
        {
            yield return new WaitUntil(() => selection.TrueForAll(c => c.State == CardState.Revealed));

            var a = selection[0];
            var b = selection[1];

            if (a.Id == b.Id)
            {
                a.Match();
                b.Match();
            }
            else
            {
                yield return new WaitForSeconds(mismatchDelay);
                a.Hide();
                b.Hide();
                yield return new WaitUntil(() => a.State == CardState.Hidden && b.State == CardState.Hidden);
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
        /// Clear all cards from the board
        /// </summary>
        private void ClearBoard()
        {
            foreach (var card in spawnedCards)
            {
                if (card != null)
                {
                    card.OnCardClicked -= OnCardClicked;
                    Destroy(card.gameObject);
                }
            }
            spawnedCards.Clear();
        }

        /// <summary>
        /// Get all cards on the board
        /// </summary>
        public List<Card> GetAllCards()
        {
            return new List<Card>(spawnedCards);
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
    }
}
