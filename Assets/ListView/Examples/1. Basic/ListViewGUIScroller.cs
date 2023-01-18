using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class ListViewGUIScroller : ListViewScroller
    {
        [SerializeField]
        GUISkin m_Skin;

        void OnGUI()
        {
            GUI.skin = m_Skin;
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label("This is a basic List View. We are only extending the class in order to add the GUI." +
                "  Use the buttons below to scroll the m_List, or feel free to modify the value of Scroll Offset in" +
                " the inspector");

            if (GUILayout.Button("Scroll Next"))
                m_ListView.ScrollNext();

            if (GUILayout.Button("Scroll Previous"))
                m_ListView.ScrollPrevious();

            var scrolling = false;
            if (GUILayout.RepeatButton("Smooth Scroll Next"))
            {
                scrolling = true;
                m_ListView.scrollOffset -= m_ListView.scrollSpeed * Time.deltaTime;
            }

            if (GUILayout.RepeatButton("Smooth Scroll Previous"))
            {
                scrolling = true;
                m_ListView.scrollOffset += m_ListView.scrollSpeed * Time.deltaTime;
            }

            if (scrolling)
                m_ListView.OnScrollStarted();
            else
                m_ListView.OnScrollEnded();

            GUILayout.EndArea();
        }
    }
}
