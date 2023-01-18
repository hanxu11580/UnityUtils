using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class CubeItem : ListViewItem<CubeItemData, int>
    {
        [SerializeField]
        TextMesh m_Label;

        public override void Setup(CubeItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Label.text = datum.text;
        }
    }
}
