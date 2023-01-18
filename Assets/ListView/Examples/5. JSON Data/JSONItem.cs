using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class JSONItem : ListViewItem<JSONItemData, int>
    {
        [SerializeField]
        TextMesh m_Label;

        public override void Setup(JSONItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Label.text = datum.text;
        }
    }
}
