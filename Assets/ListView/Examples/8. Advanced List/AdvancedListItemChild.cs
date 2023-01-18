using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class AdvancedListItemChild : AdvancedListItem
    {
        [SerializeField]
        TextMesh m_Description;

        [SerializeField]
        Transform m_ModelTransform;

        public Transform model { get; private set; }

        public override void Setup(AdvancedListItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Description.text = datum.description;
        }

        public void SetModel(Transform modelTransform)
        {
            model = modelTransform;
            modelTransform.parent = m_ModelTransform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localScale = Vector3.one;
            modelTransform.localRotation = Quaternion.identity;
        }
    }
}
