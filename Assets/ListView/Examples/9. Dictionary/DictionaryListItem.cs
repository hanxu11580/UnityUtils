using System.Collections.Generic;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class DictionaryListItem : ListViewItem<DictionaryListItemData, KeyValuePair<int, int>>
    {
        [SerializeField]
        TextMesh m_Word;

        [SerializeField]
        TextMesh m_Definition;

        public override void Setup(DictionaryListItemData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);
            m_Word.text = datum.word;
            m_Definition.text = datum.definition;
        }
    }
}
