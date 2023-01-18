using System;
using UnityEngine;

namespace Unity.Labs.ListView
{
    [Serializable]
    sealed class CubeItemData : IListViewItemData<int>
    {
        [SerializeField]
        string m_Template;

        public string text { get; set; }
        public int index { get; set; }

        public string template
        {
            get { return m_Template; }
        }
    }
}
