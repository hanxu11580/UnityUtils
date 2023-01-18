using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class ListViewVerticalMouseScroller : ListViewMouseScroller
    {
        protected override void OnScroll(Vector3 position)
        {
            if (m_Scrolling)
                m_ListView.scrollOffset = m_StartOffset + m_StartPosition.y - position.y;
        }
    }
}
