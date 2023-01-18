using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class SimpleList : MonoBehaviour
    {
        [SerializeField]
        GameObject m_Prefab;

        [SerializeField]
        int m_DataOffset;

        [SerializeField]
        float m_ItemHeight = 1;

        [SerializeField]
        int m_Range = 5;

        [SerializeField]
        string[] m_Data;

        [SerializeField]
        GUISkin m_Skin;

        TextMesh[] m_Items;

        void Start()
        {
            m_Items = new TextMesh[m_Range];
            for (var i = 0; i < m_Range; i++)
            {
                var item = Instantiate(m_Prefab).GetComponent<TextMesh>();
                var itemTransform = item.transform;
                itemTransform.position = transform.position + Vector3.down * i * m_ItemHeight;
                itemTransform.parent = transform;
                m_Items[i] = item;
            }

            UpdateList();
        }

        void UpdateList()
        {
            var dataLength = m_Data.Length;
            for (var i = 0; i < m_Range; i++)
            {
                var dataIndex = i + m_DataOffset;
                var item = m_Items[i];
                if (dataIndex >= 0 && dataIndex < dataLength)
                    item.text = m_Data[dataIndex];
                else
                    item.text = "";
            }
        }

        void OnGUI()
        {
            GUI.skin = m_Skin;
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label("This is an overly simplistic list view. Click the buttons below to scroll, or modify Data Offset in the inspector");
            if (GUILayout.Button("Scroll Next"))
                ScrollNext();

            if (GUILayout.Button("Scroll Previous"))
                ScrollPrevious();

            GUILayout.EndArea();
        }

        void ScrollNext()
        {
            m_DataOffset++;
            UpdateList();
        }

        void ScrollPrevious()
        {
            m_DataOffset--;
            UpdateList();
        }
    }
}
