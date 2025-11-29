using System;
using CardMatch.Data;
using UnityEngine;
using DG.Tweening;

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
        
        [Header("Animation")]
        [SerializeField] private float flipDuration = 0.3f;
        [SerializeField] private Ease flipEase = Ease.InOutQuad;
        private Sequence flipSequence;

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

        private Color dimColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);

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

            // Set label text directly; fonts and fallbacks will provide glyphs
            if (labelText != null)
            {
                labelText.text = labelSymbol ?? string.Empty;
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
            if (currentState == CardState.Locked)
                return;

            bool isRevealed = currentState == CardState.Revealed || currentState == CardState.Matched;

            if (cardFront != null)
                cardFront.SetActive(isRevealed);

            if (cardBack != null)
                cardBack.SetActive(!isRevealed);

            // Dim matched cards
            if (currentState == CardState.Matched)
            {
                // Dim matched cards (grey/transparent black)
                if (labelText != null) 
                    labelText.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
            }
            else
            {
                // Use black for simple symbols (Noto Sans glyphs)
                if (labelText != null) 
                    labelText.color = Color.black;
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
                FlipTo(true, CardState.Revealed);
            }
        }

        /// <summary>
        /// Hide the card (flip to back)
        /// </summary>
        public void Hide()
        {
            if (currentState == CardState.Revealed)
            {
                FlipTo(false, CardState.Hidden);
            }
        }

        private void FlipTo(bool showFront, CardState targetState)
        {
            if (flipSequence != null && flipSequence.active)
            {
                flipSequence.Kill();
            }

            SetState(CardState.Locked);

            flipSequence = DOTween.Sequence();
            flipSequence.Append(transform.DOScaleX(0f, flipDuration * 0.5f).SetEase(flipEase));
            flipSequence.AppendCallback(() =>
            {
                if (cardFront != null) cardFront.SetActive(showFront);
                if (cardBack != null) cardBack.SetActive(!showFront);
            });
            flipSequence.Append(transform.DOScaleX(1f, flipDuration * 0.5f).SetEase(flipEase));
            flipSequence.OnComplete(() =>
            {
                SetState(targetState);
            });
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
