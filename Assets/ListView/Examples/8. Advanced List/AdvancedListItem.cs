using UnityEngine;

namespace Unity.Labs.ListView
{
    class AdvancedListItem : NestedListViewItem<AdvancedListItemData, int>
    {
        [SerializeField]
        TextMesh m_Title;

        public override void Setup(AdvancedListItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Title.text = datum.title;
        }
    }
}
