using System.Collections.Generic;
using UnityEngine;

//Uses JSONObject http://u3d.as/1Rh

namespace Unity.Labs.ListView
{
    sealed class JSONList : ListViewController<JSONItemData, JSONItem, int>
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
                data = new List<JSONItemData>(length);
                for (var i = 0; i < length; i++)
                {
                    data.Add(new JSONItemData(obj[i], i, m_DefaultTemplate));
                }
            }
        }
    }
}
