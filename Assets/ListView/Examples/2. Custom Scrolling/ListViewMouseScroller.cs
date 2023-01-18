using UnityEngine;

namespace Unity.Labs.ListView
{
    class ListViewMouseScroller : ListViewScroller
    {
        [SerializeField]
        float m_ScrollWheelSpeed = 1;

        float m_ListDepth;

        bool m_ScrollWheelScrolling;

        Camera m_MainCamera;

        void Start()
        {
            m_MainCamera = Camera.main;
            if (m_MainCamera == null)
                enabled = false;
        }

        void Update()
        {
            var screenPoint = Input.mousePosition;
            var begin = false;
            var mouseHeld = Input.GetMouseButton(0);
            if (mouseHeld)
            {
                RaycastHit hit;
                if (Physics.Raycast(m_MainCamera.ScreenPointToRay(screenPoint), out hit))
                {
                    var item = hit.collider.GetComponent<IListViewItem>();
                    if (item != null)
                    {
                        m_ListDepth = (hit.point - m_MainCamera.transform.position).magnitude;
                        begin = true;
                    }
                }
            }

            screenPoint.z = m_ListDepth;
            var worldPoint = m_MainCamera.ScreenToWorldPoint(screenPoint);
            if (begin)
                OnScrollStarted(worldPoint);

            OnScroll(worldPoint);

            if (!mouseHeld)
                OnScrollEnded();

            var scrollWheelDelta = Input.mouseScrollDelta.y;
            if (Mathf.Approximately(scrollWheelDelta, 0))
            {
                if (m_ScrollWheelScrolling)
                {
                    m_ListView.OnScrollEnded();
                    m_ScrollWheelScrolling = false;
                }
            }
            else
            {
                m_ListView.scrollOffset += scrollWheelDelta * m_ScrollWheelSpeed;
                m_ListView.OnScrollStarted();
                m_ScrollWheelScrolling = true;
            }
        }
    }
}
