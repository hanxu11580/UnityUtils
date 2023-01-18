using System.Linq;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class CubeList : ListViewController<CubeItemData, CubeItem, int>
    {
        [SerializeField]
        float m_Range;

        [SerializeField]
        CubeItemData[] m_CubeData;

        protected override void Awake()
        {
            base.Awake();

            size = Vector3.forward * m_Range;

            var length = m_CubeData.Length;
            for (var i = 0; i < length; i++)
            {
                m_CubeData[i].index = i;
            }

            data = m_CubeData.ToList();
        }

        protected override void Start()
        {
            base.Start();

            var count = data.Count;
            for (var i = 0; i < count; i++)
            {
                data[i].text = i.ToString();
            }
        }
    }
}
