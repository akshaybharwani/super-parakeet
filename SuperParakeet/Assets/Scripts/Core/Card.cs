using System;
using CardMatch.Data;
using UnityEngine;

namespace CardMatch.Core
{
    /// <summary>
    /// Represents a single card in the game
    /// Handles state, visuals, and user interaction
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer backRenderer;
        [SerializeField] private GameObject cardFront;
        [SerializeField] private GameObject cardBack;
        [SerializeField] private TMPro.TextMeshPro labelText;

        private CardData data;
        private CardState currentState = CardState.Hidden;
        private int gridX;
        private int gridY;

        // Public properties
        public int Id => data != null ? data.Id : -1;
        public CardState State => currentState;
        public int GridX => gridX;
        public int GridY => gridY;
        public CardData Data => data;
        public TMPro.TextMeshPro LabelText => labelText;

        // Events
        public event Action<Card> OnCardClicked;

        /// <summary>
        /// Initialize card with data, position, and label
        /// </summary>
        public void Initialize(CardData data, int gridX, int gridY, Vector3 localPosition, string labelSymbol)
        {
            this.data = data;
            this.gridX = gridX;
            this.gridY = gridY;

            // Set position
            transform.localPosition = localPosition;

            // Set label text
            if (labelText != null)
            {
                labelText.text = labelSymbol;
            }
            else
            {
                Debug.LogWarning($"[Card] No TextMeshPro component assigned on card at ({gridX},{gridY})");
            }

            SetState(CardState.Hidden);
        }

        /// <summary>
        /// Change card state and update visuals
        /// </summary>
        public void SetState(CardState newState)
        {
            currentState = newState;
            UpdateVisuals();
        }

        /// <summary>
        /// Update visual representation based on current state
        /// </summary>
        private void UpdateVisuals()
        {
            bool isRevealed = currentState == CardState.Revealed || currentState == CardState.Matched;
            
            if (cardFront != null) 
                cardFront.SetActive(isRevealed);
            
            if (cardBack != null) 
                cardBack.SetActive(!isRevealed);

            // Dim matched cards
            if (currentState == CardState.Matched)
            {
                Color dimColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                if (labelText != null) 
                    labelText.color = dimColor;
            }
            else
            {
                if (labelText != null) 
                    labelText.color = Color.white;
            }
        }

        /// <summary>
        /// Handle card click/tap
        /// </summary>
        public void OnClick()
        {
            if (currentState == CardState.Hidden)
            {
                OnCardClicked?.Invoke(this);
            }
        }

        /// <summary>
        /// Reveal the card (flip to front)
        /// </summary>
        public void Reveal()
        {
            if (currentState == CardState.Hidden)
            {
                // TODO: Trigger flip animation (card back -> card front)
                SetState(CardState.Revealed);
            }
        }

        /// <summary>
        /// Hide the card (flip to back)
        /// </summary>
        public void Hide()
        {
            if (currentState == CardState.Revealed)
            {
                // TODO: Trigger flip animation (card front -> card back)
                SetState(CardState.Hidden);
            }
        }

        /// <summary>
        /// Mark card as matched
        /// </summary>
        public void Match()
        {
            // TODO: Optional - play match success animation/effect
            SetState(CardState.Matched);
        }

        /// <summary>
        /// Lock card during animations
        /// </summary>
        public void Lock()
        {
            SetState(CardState.Locked);
        }
    }
}