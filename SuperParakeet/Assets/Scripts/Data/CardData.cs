using UnityEngine;

namespace CardMatch.Data
{
    /// <summary>
    /// ScriptableObject to store card configuration data
    /// </summary>
    [CreateAssetMenu(fileName = "NewCardData", menuName = "CardMatch/Card Data")]
    public class CardData : ScriptableObject
    {
        [SerializeField] private int id;

        public int Id => id;

        /// <summary>
        /// Create a runtime instance of CardData
        /// </summary>
        public static CardData Create(int id)
        {
            var data = CreateInstance<CardData>();
            data.id = id;
            return data;
        }
    }
}