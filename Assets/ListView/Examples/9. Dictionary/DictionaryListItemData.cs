using System.Collections.Generic;

namespace Unity.Labs.ListView
{
    sealed class DictionaryListItemData : IListViewItemData<KeyValuePair<int, int>>
    {
        readonly string m_Word;
        readonly string m_Definition;
        readonly string m_Template;
        readonly KeyValuePair<int, int> m_Index;

        public string word
        {
            get { return m_Word; }
        }

        public string definition
        {
            get { return m_Definition; }
        }

        public string template
        {
            get { return m_Template; }
        }

        public KeyValuePair<int, int> index
        {
            get { return m_Index; }
        }

        public DictionaryListItemData(string word, string definition, KeyValuePair<int, int> index, string template)
        {
            m_Word = word;
            m_Definition = definition;
            m_Index = index;
            m_Template = template;
        }
    }
}
