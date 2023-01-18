using System.Linq;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class WrappingList : ListViewController<CubeItemData, CubeItem, int>
    {
        [SerializeField]
        float m_Range;

        [SerializeField]
        CubeItemData[] m_CubeData;

        [SerializeField]
        GUISkin m_Skin;

        protected override void Awake()
        {
            base.Awake();

            size = Vector3.forward * m_Range;
            SetupData();
        }

        void SetupData()
        {
            var length = m_CubeData.Length;
            for (var i = 0; i < length; i++)
            {
                m_CubeData[i].index = i;
            }

            data = m_CubeData.ToList();
            for (var i = 0; i < length; i++)
            {
                data[i].text = i.ToString();
            }
        }

        void OnGUI()
        {
            GUI.skin = m_Skin;

            if (GUILayout.Button("Scroll Previous"))
                ScrollPrevious();

            if (GUILayout.Button("Scroll Next"))
                ScrollNext();
        }

        // Override ComputeConditions to disable limiting on scrollOffset
        protected override void ComputeConditions()
        {
            if (m_CubeData.Length != m_Data.Count)
                SetupData();

            m_StartPosition = (m_Extents.z - m_ItemSize.z * 0.5f) * Vector3.forward;
        }

        public override void OnScrollEnded()
        {
            m_Scrolling = false;
        }

        protected override void UpdateItems()
        {
            var doneSettling = true;
            var wrapped = false;
            var offset = 0f;
            var order = 0;
            var count = m_Data.Count;
            var itemWidth = m_ItemSize.z;
            var totalWidth = count * itemWidth;
            var visibleWidth = m_Size.z;
            var maxVisibleSize = Mathf.CeilToInt(visibleWidth / itemWidth) * itemWidth;

            var wrappedOffset = scrollOffset % totalWidth;
            if (wrappedOffset > maxVisibleSize)
                wrappedOffset -= totalWidth;

            if (maxVisibleSize > totalWidth)
                maxVisibleSize = totalWidth;

            for (var i = 0; i < count; i++)
            {
                var datum = m_Data[i];
                var localOffset = offset + wrappedOffset;

                if (localOffset >= maxVisibleSize && wrappedOffset >= 0 && !wrapped)
                {
                    var remaining = Mathf.FloorToInt(wrappedOffset / itemWidth);
                    offset = -itemWidth * (count + remaining) + maxVisibleSize;
                    localOffset = offset + wrappedOffset;
                    wrapped = true;
                }

                if (localOffset + itemWidth < 0 || localOffset > visibleWidth)
                    Recycle(datum.index);
                else
                    UpdateVisibleItem(datum, order++, localOffset, ref doneSettling);

                if (localOffset < -totalWidth + maxVisibleSize && wrappedOffset < 0)
                    UpdateVisibleItem(datum, order++, wrappedOffset + totalWidth + i * itemWidth, ref doneSettling);

                offset += itemWidth;
            }

            if (m_Settling && doneSettling)
                EndSettling();
        }
    }
}
