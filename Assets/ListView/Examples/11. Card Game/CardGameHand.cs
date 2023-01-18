using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class CardGameHand : ListViewController<CardData, Card, int>
    {
        [SerializeField]
        float m_Radius = 0.25f;

        [SerializeField]
        float m_Interpolate = 15f;

        [SerializeField]
        float m_StackOffset = 0.01f;

        [SerializeField]
        int m_HandSize = 5;

        [SerializeField]
        float m_IndicatorTime = 0.5f;

        [SerializeField]
        CardGameList m_Controller;

        [SerializeField]
        Renderer m_Indicator;

        float m_CardDegrees, m_CardsOffset;

        protected override void Start()
        {
            base.Start();
            data = new List<CardData>(m_HandSize);
            for (var i = 0; i < m_HandSize; i++)
            {
                CardData cardData;
                var card = m_Controller.DrawCard(out cardData);
                card.transform.parent = transform;
                m_ListItems[cardData.index] = card;
                data.Add(cardData);
            }
        }

        protected override void ComputeConditions()
        {
            m_CardDegrees = Mathf.Atan(m_ItemSize.x / m_Radius) * Mathf.Rad2Deg;
            m_CardsOffset = m_CardDegrees * (data.Count - 1) * 0.5f;
        }

        protected override void UpdateItems()
        {
            var position = transform.position;
            var rotation = transform.rotation;
            var itemHeight = m_ItemSize.z;
            DebugDrawCircle(m_Radius - itemHeight * 0.5f, 24, position, rotation);
            DebugDrawCircle(m_Radius + itemHeight * 0.5f, 24, position, rotation);
            var doneSettling = true;
            var count = data.Count;
            for (var i = 0; i < count; i++)
            {
                UpdateVisibleItem(data[i], i, i, ref doneSettling);
            }
        }

        protected override void UpdateItem(IListViewItem item, int offset, float f, ref bool doneSettling)
        {
            var sliceRotation = Quaternion.AngleAxis(90 - m_CardsOffset + m_CardDegrees * offset, Vector3.up);
            item.localPosition = Vector3.Lerp(item.localPosition, sliceRotation * (Vector3.left * m_Radius)
                + Vector3.up * m_StackOffset * offset, m_Interpolate * Time.deltaTime);
            item.localRotation = Quaternion.Lerp(item.localRotation, sliceRotation * Quaternion.AngleAxis(90, Vector3.down),
                m_Interpolate * Time.deltaTime);
        }

        public static void DebugDrawCircle(float radius, int slices, Vector3 center)
        {
            DebugDrawCircle(radius, slices, center, Quaternion.identity);
        }

        public static void DebugDrawCircle(float radius, int slices, Vector3 center, Quaternion orientation)
        {
            var forward = Vector3.forward * radius;
            for (var i = 0; i < slices; i++)
            {
                Debug.DrawLine(
                    center + orientation * Quaternion.AngleAxis((float)360 * i / slices, Vector3.up) * forward,
                    center + orientation * Quaternion.AngleAxis((float)360 * (i + 1) / slices, Vector3.up) * forward);
            }
        }

        public void DrawCard(Card item)
        {
            if (data.Count < m_HandSize)
            {
                var cardData = item.data;
                data.Add(cardData);
                m_ListItems[cardData.index] = item;
                m_Controller.RemoveCardFromDeck(cardData);
                item.transform.parent = transform;
            }
            else
            {
                Indicate();
                Debug.Log("Can't draw card, hand is full!");
            }
        }

        public void ReturnCard(Card item)
        {
            var cardData = item.data;
            if (data.Contains(cardData))
            {
                data.Remove(cardData);
                var index = cardData.index;
                var card = m_ListItems[index];
                m_ListItems.Remove(index);
                m_Controller.AddCardToDeck(card);
            }
            else
            {
                Indicate();
                Debug.Log("Something went wrong... This card is not in your hand");
            }
        }

        void Indicate()
        {
            StartCoroutine(DoIndicate());
        }

        IEnumerator DoIndicate()
        {
            m_Indicator.enabled = true;
            yield return new WaitForSeconds(m_IndicatorTime);
            m_Indicator.enabled = false;
        }
    }
}
