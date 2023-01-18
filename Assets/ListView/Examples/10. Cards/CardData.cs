using System;

namespace Unity.Labs.ListView
{
    [Serializable]
    sealed class CardData : IListViewItemData<int>
    {
        readonly string m_Value;
        readonly Card.Suit m_Suit;
        readonly int m_Index;
        readonly string m_Template;

        public string value { get { return m_Value; } }
        public Card.Suit suit { get { return m_Suit; } }
        public int index { get { return m_Index; } }
        public string template { get { return m_Template; } }

        public CardData(string value, Card.Suit suit, int index, string template)
        {
            m_Value = value;
            m_Suit = suit;
            m_Index = index;
            m_Template = template;
        }
    }
}
