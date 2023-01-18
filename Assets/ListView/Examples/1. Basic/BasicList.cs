using System.Linq;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class BasicList : ListViewController<BasicItemData, BasicItem, int>
    {
        [SerializeField]
        float m_Range;

        [SerializeField]
        BasicItemData[] m_BasicData;

        protected override void Awake()
        {
            base.Awake();

            size = Vector3.forward * m_Range;

            for (var i = 0; i < m_BasicData.Length; i++)
            {
                m_BasicData[i].index = i;
            }

            m_Data = m_BasicData.ToList();
        }
    }
}
