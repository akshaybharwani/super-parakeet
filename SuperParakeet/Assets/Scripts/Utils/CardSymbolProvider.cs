namespace CardMatch.Utils
{
    /// <summary>
    /// Provides unicode symbols/icons for card labels
    /// </summary>
    public static class CardSymbolProvider
    {
        private static readonly string[] symbols = new string[]
        {
            "★",
            "♠",
            "♥",
            "♦",
            "♣",
            "●",
            "■",
            "▲",
            "◆",
            "✦",
            "❒",
            "☀",
            "☁",
            "☂",
            "✈",
            "⚡",
            "❄",
            "☘",
            "♔",
            "♕",
            "♖",
            "♗",
            "♘",
            "♙",
            "✪",
            "◉",
            "▣",
            "△",
            "◈",
            "✚"
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
