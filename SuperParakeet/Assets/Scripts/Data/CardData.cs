using UnityEngine;

namespace CardMatch.Data
{
    /// <summary>
    /// ScriptableObject to store card configuration data
    /// </summary>
    [CreateAssetMenu(fileName = "NewCardData", menuName = "CardMatch/Card Data")]
    public class CardData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _cardName;

        public int Id => _id;
        public Sprite Icon => _icon;
        public string CardName => _cardName;
        public static CardData Create(int id, Sprite icon, string name = "")
        {
            var data = CreateInstance<CardData>();
            data._id = id;
            data._icon = icon;
            data._cardName = name;
            return data;
        }
    }
}