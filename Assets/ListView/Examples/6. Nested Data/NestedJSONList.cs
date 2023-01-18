using System.Collections.Generic;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class NestedJSONList : NestedListViewController<NestedJSONItemData, NestedJSONItem, int>
    {
        [SerializeField]
        string m_DataFile;

        [SerializeField]
        string m_DefaultTemplate;

        [SerializeField]
        float m_Range;

        protected override void Awake()
        {
            base.Awake();
            size = Vector3.forward * m_Range;
        }

        protected override void Start()
        {
            base.Start();

            var text = Resources.Load<TextAsset>(m_DataFile);
            if (text)
            {
                var obj = new JSONObject(text.text);
                var length = obj.Count;
                m_Data = new List<NestedJSONItemData>(length);
                var index = 0;
                for (var i = 0; i < length; i++)
                {
                    m_Data.Add(new NestedJSONItemData(obj[i], m_DefaultTemplate, ref index));
                }
            }
        }
    }
}
