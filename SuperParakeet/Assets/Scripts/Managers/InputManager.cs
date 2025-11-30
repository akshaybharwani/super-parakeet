using UnityEngine;
using CardMatch.Core;

namespace CardMatch.Managers
{
    /// <summary>
    /// Manages user input for both desktop (mouse) and mobile (touch)
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask cardLayer = -1; // Default to everything

        private bool inputEnabled = true;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (!inputEnabled) return;
            
            if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance?.CurrentState != GameState.Paused)
            {
                // Handle back button on mobile or Escape key on desktop
                GameManager.Instance.HUDCanvas.OnQuitClicked();
            }

            // Check game state
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            // Handle input
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput(Input.mousePosition);
            }
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    HandleInput(touch.position);
                }
            }
        }

        /// <summary>
        /// Process input at screen position
        /// </summary>
        private void HandleInput(Vector3 screenPosition)
        {
            if (mainCamera == null) return;

            // Raycast for 2D objects
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, cardLayer);

            if (hit.collider != null)
            {
                Card card = hit.collider.GetComponent<Card>();
                if (card != null)
                {
                    card.OnClick();
                    // Debug.Log($"[InputManager] Card clicked via raycast: ID={card.Id}");
                }
            }
        }

        /// <summary>
        /// Enable or disable input processing
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            // Debug.Log($"[InputManager] Input {(enabled ? "enabled" : "disabled")}");
        }
    }
}