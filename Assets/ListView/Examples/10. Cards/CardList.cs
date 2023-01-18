using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Images sourced from http://web.stanford.edu/~jlewis8/cs148/pokerscene/

namespace Unity.Labs.ListView
{
    sealed class CardList : ListViewController<CardData, Card, int>
    {
        [SerializeField]
        string m_DefaultTemplate = "Card";

        [SerializeField]
        float m_RecycleDuration = 0.3f;

        [SerializeField]
        bool m_AutoScroll;

        [SerializeField]
        Transform m_LeftDeck;

        [SerializeField]
        Transform m_RightDeck;

        [SerializeField]
        float m_Range;

        protected override float listHeight
        {
            get { return m_Data.Count * m_ItemSize.x; }
        }

        protected override void Awake()
        {
            base.Awake();
            size = m_Range * Vector3.right;
        }

        protected override void Start()
        {
            base.Start();

            var deck = new List<CardData>(52);
            Card.FillDeck(deck, m_DefaultTemplate);
            m_Data = Card.Shuffle(deck);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(m_Range, m_ItemSize.y, m_ItemSize.z));
        }

        protected override void ComputeConditions()
        {
            base.ComputeConditions();
            m_StartPosition = (m_Extents.x - m_ItemSize.x * 0.5f) * Vector3.left;
        }

        protected override void UpdateItems()
        {
            if (m_AutoScroll)
            {
                scrollOffset -= scrollSpeed * Time.deltaTime;
                if (-scrollOffset > listHeight || scrollOffset >= 0)
                    scrollSpeed *= -1;
            }

            var doneSettling = true;
            var itemWidth = m_ItemSize.x;
            var listWidth = m_Size.x;
            var offset = 0f;
            var count = m_Data.Count;
            for (var i = 0; i < count; i++)
            {
                var datum = m_Data[i];
                var localOffset = offset + scrollOffset;
                if (localOffset + itemWidth < 0)
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

        void ExtremeLeft(CardData datum)
        {
            RecycleItemAnimated(datum, m_LeftDeck);
        }

        void ExtremeRight(CardData datum)
        {
            RecycleItemAnimated(datum, m_RightDeck);
        }

        void ListMiddle(CardData datum, int order, float offset, ref bool doneSettling)
        {
            Card card;
            var index = datum.index;
            if (!m_ListItems.TryGetValue(index, out card))
            {
                GetNewItem(datum, out card);
                var cardTransform = card.transform;

                if (m_ScrollDelta < 0 || Mathf.Approximately(m_ScrollOffset, 0))
                {
                    var rightDeckTransform = m_RightDeck.transform;
                    cardTransform.position = rightDeckTransform.position;
                    cardTransform.rotation = rightDeckTransform.rotation;
                }
                else
                {
                    var leftDeckTransform = m_LeftDeck.transform;
                    cardTransform.position = leftDeckTransform.position;
                    cardTransform.rotation = leftDeckTransform.rotation;
                }

                StartSettling();
            }

            UpdateItem(card, order, offset, ref doneSettling);
        }

        void RecycleItemAnimated(CardData datum, Transform destination)
        {
            var dataIndex = datum.index;
            Card card;
            if (!m_ListItems.TryGetValue(dataIndex, out card))
                return;

            StartCoroutine(RecycleAnimation(card, datum.template, destination, m_RecycleDuration));
            m_ListItems.Remove(dataIndex);
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
            var targetRotation = Quaternion.identity;
            UpdateItemTransform(item, order, targetPosition, targetRotation, false, ref doneSettling);
        }
    }
}
