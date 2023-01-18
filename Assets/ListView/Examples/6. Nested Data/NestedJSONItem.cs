using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class NestedJSONItem : NestedListViewItem<NestedJSONItemData, int>
    {
        [SerializeField]
        TextMesh m_Label;

        public override void Setup(NestedJSONItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Label.text = datum.text;
        }
    }
}
