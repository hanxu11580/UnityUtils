using System.Collections.Generic;

namespace Unity.Labs.ListView
{
    sealed class AdvancedListItemData : NestedListViewItemData<AdvancedListItemData, int>
    {
        readonly int m_Index;
        readonly string m_Template;
        readonly string m_Title;
        readonly string m_Description;
        readonly string m_Model;

        public string title { get { return m_Title; } }
        public string description { get { return m_Description; } }
        public string model { get { return m_Model; } }
        public override int index { get { return m_Index; } }
        public override string template { get { return m_Template; } }

        public AdvancedListItemData(JSONObject obj, ref int index)
        {
            obj.GetField(ref m_Title, "title");
            obj.GetField(ref m_Description, "description");
            obj.GetField(ref m_Model, "model");
            m_Template = obj.GetField("template").str;
            m_Index = index;
            var indexClosure = index + 1;
            obj.GetField("children", delegate(JSONObject childrenJSON)
            {
                var count = childrenJSON.Count;
                children = new List<AdvancedListItemData>(count);
                for (var i = 0; i < count; i++)
                {
                    var child = new AdvancedListItemData(childrenJSON[i], ref indexClosure);
                    children.Add(child);
                }
            });

            index = indexClosure;
        }
    }
}
