using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class AdvancedListScroller : ListViewScroller
    {
        [SerializeField]
        float m_ScrollThreshold = 0.2f;

        [SerializeField]
        float m_ScrollWheelSpeed = 1;

        float m_ListDepth;

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

            if (Mathf.Abs(m_StartPosition.y - worldPoint.y) < m_ScrollThreshold)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(m_MainCamera.ScreenPointToRay(screenPoint), out hit))
                    {
                        var item = hit.collider.GetComponent<AdvancedListItem>();
                        if (item)
                            item.ToggleExpanded();
                    }
                }
            }

            if (!mouseHeld)
                OnScrollEnded();

            m_ListView.scrollOffset += Input.mouseScrollDelta.y * m_ScrollWheelSpeed;
        }

        protected override void OnScroll(Vector3 position)
        {
            if (m_Scrolling)
                m_ListView.scrollOffset = m_StartOffset + m_StartPosition.y - position.y;
        }
    }
}
