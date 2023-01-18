using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class ListViewExpander : MonoBehaviour
    {
        Camera m_MainCamera;

        void Start()
        {
            m_MainCamera = Camera.main;
            if (m_MainCamera == null)
                enabled = false;
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(m_MainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    var item = hit.collider.GetComponent<INestedListViewItem>();
                    if (item != null)
                        item.ToggleExpanded();
                }
            }
        }
    }
}
