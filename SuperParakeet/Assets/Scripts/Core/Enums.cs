namespace CardMatch.Core
{
    /// <summary>
    /// Defines the overall game state
    /// </summary>
    public enum GameState
    {
        Initializing,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// Defines the state of individual cards
    /// </summary>
    public enum CardState
    {
        Hidden,      // Face down
        Revealed,    // Face up, waiting for match
        Matched,     // Successfully matched
        Locked       // Temporarily disabled during animations
    }
}