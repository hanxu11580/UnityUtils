using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class WebItem : ListViewItem<WebItemData, string>
    {
        [SerializeField]
        TextMesh m_Label;

        public override void Setup(WebItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Label.text = datum.text;
        }
    }
}
