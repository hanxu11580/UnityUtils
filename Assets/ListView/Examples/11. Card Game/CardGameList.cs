using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class CardGameList : ListViewController<CardData, Card, int>
    {
        static readonly Quaternion k_TargetRotation = Quaternion.identity;

        [SerializeField]
        string m_DefaultTemplate = "Card";

        [SerializeField]
        float m_RecycleDuration = 0.3f;

        [SerializeField]
        int m_DealMax = 5;

        [SerializeField]
        Transform m_Deck;

        [SerializeField]
        float m_Range;

        protected override float listHeight
        {
            get { return m_Data == null ? 0 : m_Data.Count * m_ItemSize.x; }
        }

        protected override void Start()
        {
            base.Start();

            var deck = new List<CardData>(52);
            Card.FillDeck(deck, m_DefaultTemplate);
            m_Data = Card.Shuffle(deck);

            m_Range = 0;
            m_ScrollOffset = m_ItemSize.x * 0.5f;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + Vector3.left * (m_ItemSize.x * m_DealMax - m_Range) * 0.5f,
                new Vector3(m_Range, m_ItemSize.y, m_ItemSize.z));
        }

        protected override void UpdateItems()
        {
            var itemWidth = m_ItemSize.x;
            m_StartPosition = Vector3.left * itemWidth * m_DealMax * 0.5f;
            size = m_Range * Vector3.right;
            var listWidth = m_Size.x;
            var doneSettling = true;
            var offset = 0f;
            var count = data.Count;
            for (var i = 0; i < count; i++)
            {
                var datum = data[i];
                var localOffset = offset + scrollOffset;
                if (localOffset < 0)
                    ExtremeLeft(datum);
                else if (localOffset > listWidth)
                    ExtremeRight(datum);
                else
                    ListMiddle(datum, i, localOffset, ref doneSettling);

                offset += itemWidth;
            }

            if (m_Settling && doneSettling)
                EndSettling();
        }

        // Override to suppress scrollback behavior
        public override void OnScrollEnded()
        {
        }

        void ExtremeLeft(CardData datum)
        {
            RecycleItemAnimated(datum, m_Deck);
        }

        void ExtremeRight(CardData datum)
        {
            RecycleItemAnimated(datum, m_Deck);
        }

        void ListMiddle(CardData datum, int order, float offset, ref bool doneSettling)
        {
            Card card;
            if (!m_ListItems.TryGetValue(datum.index, out card))
            {
                GetNewItem(datum, out card);
                var cardTransform = card.transform;
                cardTransform.position = m_Deck.transform.position;
                cardTransform.rotation = m_Deck.transform.rotation;
            }

            UpdateItem(card, order, offset, ref doneSettling);
        }

        protected override bool GetNewItem(CardData datum, out Card item)
        {
            var instantiated = base.GetNewItem(datum, out item);
            if (!instantiated)
                item.boxCollider.enabled = true;

            return instantiated;
        }

        void RecycleItemAnimated(CardData datum, Transform destination)
        {
            Card card;
            var index = datum.index;
            if (!m_ListItems.TryGetValue(index, out card))
                return;

            m_ListItems.Remove(index);

            card.GetComponent<BoxCollider>().enabled = false; //Disable collider so we can't click the card during the animation
            StartCoroutine(RecycleAnimation(card, datum.template, destination, m_RecycleDuration));
        }

        IEnumerator RecycleAnimation(Card card, string template, Transform destination, float duration)
        {
            var start = Time.time;
            var cardTransform = card.transform;
            var startRot = cardTransform.rotation;
            var startPos = cardTransform.position;
            var elapsed = Time.time - start;
            var destinationRotation = destination.rotation;
            var destinationPosition = destination.position;
            while (elapsed < duration)
            {
                cardTransform.rotation = Quaternion.Lerp(startRot, destinationRotation, elapsed / duration);
                cardTransform.position = Vector3.Lerp(startPos, destinationPosition, elapsed / duration);
                yield return null;
                elapsed = Time.time - start;
            }

            cardTransform.rotation = destinationRotation;
            cardTransform.position = destinationPosition;
            RecycleItem(template, card);
        }

        protected override void UpdateItem(IListViewItem item, int order, float offset, ref bool doneSettling)
        {
            var targetPosition = m_StartPosition + offset * Vector3.right;
            UpdateItemTransform(item, order, targetPosition, k_TargetRotation, false, ref doneSettling);
        }

        void RecycleCard(CardData datum)
        {
            RecycleItemAnimated(datum, m_Deck);
        }

        public Card DrawCard(out CardData cardData)
        {
            if (data.Count == 0)
            {
                Debug.Log("Out of Cards");
                cardData = null;
                return null;
            }

            cardData = data.Last();
            data.Remove(cardData);

            Card card;
            var index = cardData.index;
            if (m_ListItems.TryGetValue(index, out card))
                m_ListItems.Remove(index);
            else
                GetNewItem(cardData, out card);

            return card;
        }

        public void RemoveCardFromDeck(CardData cardData)
        {
            data.Remove(cardData);
            m_ListItems.Remove(cardData.index);
            m_Settling = true;
            if (m_Range > 0)
                m_Range -= m_ItemSize.x;
        }

        public void AddCardToDeck(Card card)
        {
            var cardData = card.data;
            data.Add(cardData);

            m_ListItems[cardData.index] = card;
            card.transform.parent = transform;
            RecycleCard(cardData);
        }

        public void Deal()
        {
            var itemWidth = m_ItemSize.x;
            m_Range += itemWidth;
            if (m_Range >= itemWidth * (m_DealMax + 1))
            {
                scrollOffset -= itemWidth * m_DealMax;
                m_Range = 0;
            }

            if (-scrollOffset >= (data.Count - m_DealMax) * itemWidth)
            {
                m_Data = Card.Shuffle(m_Data);
                scrollOffset = itemWidth * 0.5f;
            }
        }
    }
}
