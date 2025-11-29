namespace CardMatch.Utils
{
    /// <summary>
    /// Provides unicode symbols/icons for card labels
    /// </summary>
    public static class CardSymbolProvider
    {
        private static readonly string[] symbols = new string[]
        {
            "★",    // Star
            "♠",    // Spade
            "♥",    // Heart
            "♦",    // Diamond
            "♣",    // Club
            "●",    // Circle
            "■",    // Square
            "▲",    // Triangle
            "◆",    // Diamond2
            "✦",    // Star2
            "♪",    // Music note
            "☀",    // Sun
            "☁",    // Cloud
            "☂",    // Umbrella
            "✈",    // Plane
            "⚡",    // Lightning
            "❄",    // Snowflake
            "☘",    // Clover
            "⚽",    // Soccer ball
            "♔",    // King
            "♕",    // Queen
            "♖",    // Rook
            "♗",    // Bishop
            "♘",    // Knight
            "♙",    // Pawn
            "✪",    // Star3
            "◉",    // Circle2
            "▣",    // Square2
            "△",    // Triangle2
            "◈"     // Diamond3
        };

        /// <summary>
        /// Get symbol for a specific card ID
        /// </summary>
        public static string GetSymbol(int cardId)
        {
            return symbols[cardId % symbols.Length];
        }

        /// <summary>
        /// Get total number of available symbols
        /// </summary>
        public static int SymbolCount => symbols.Length;
    }
}