using System;
using UnityEngine;

namespace Unity.Labs.ListView
{
    [Serializable]
    sealed class BasicItemData : IListViewItemData<int>
    {
        [SerializeField]
        string m_Template;

        public int index { get; set; }
        public string template
        {
            get { return m_Template; }
        }
    }
}
