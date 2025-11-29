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
        [SerializeField] private SpriteRenderer _iconRenderer;
        [SerializeField] private SpriteRenderer _backRenderer;
        [SerializeField] private GameObject _cardFront;
        [SerializeField] private GameObject _cardBack;

        private CardData _data;
        private CardState _currentState = CardState.Hidden;
        private int _gridX;
        private int _gridY;

        // Public properties
        public int Id => _data != null ? _data.Id : -1;
        public CardState State => _currentState;
        public int GridX => _gridX;
        public int GridY => _gridY;
        public CardData Data => _data;

        // Events
        public event Action<Card> OnCardClicked;

        /// <summary>
        /// Initialize card with data and position
        /// </summary>
        public void Initialize(CardData data, int gridX, int gridY)
        {
            _data = data;
            _gridX = gridX;
            _gridY = gridY;
            
            if (_iconRenderer != null && data.Icon != null)
            {
                _iconRenderer.sprite = data.Icon;
            }
            
            SetState(CardState.Hidden);
        }

        /// <summary>
        /// Change card state and update visuals
        /// </summary>
        public void SetState(CardState newState)
        {
            _currentState = newState;
            UpdateVisuals();
        }

        /// <summary>
        /// Update visual representation based on current state
        /// </summary>
        private void UpdateVisuals()
        {
            bool isRevealed = _currentState == CardState.Revealed || _currentState == CardState.Matched;
            
            if (_cardFront != null) 
                _cardFront.SetActive(isRevealed);
            
            if (_cardBack != null) 
                _cardBack.SetActive(!isRevealed);

            // Dim matched cards
            if (_currentState == CardState.Matched)
            {
                Color dimColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                if (_iconRenderer != null) 
                    _iconRenderer.color = dimColor;
            }
            else
            {
                if (_iconRenderer != null) 
                    _iconRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// Handle card click/tap
        /// </summary>
        public void OnClick()
        {
            if (_currentState == CardState.Hidden)
            {
                OnCardClicked?.Invoke(this);
            }
        }

        /// <summary>
        /// Reveal the card (flip to front)
        /// </summary>
        public void Reveal()
        {
            if (_currentState == CardState.Hidden)
            {
                SetState(CardState.Revealed);
            }
        }

        /// <summary>
        /// Hide the card (flip to back)
        /// </summary>
        public void Hide()
        {
            if (_currentState == CardState.Revealed)
            {
                SetState(CardState.Hidden);
            }
        }

        /// <summary>
        /// Mark card as matched
        /// </summary>
        public void Match()
        {
            SetState(CardState.Matched);
        }

        /// <summary>
        /// Lock card during animations
        /// </summary>
        public void Lock()
        {
            SetState(CardState.Locked);
        }

        private void OnMouseDown()
        {
            OnClick();
        }
    }
}