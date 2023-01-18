using System.Collections.Generic;

namespace Unity.Labs.ListView
{
    sealed class NestedJSONItemData : NestedListViewItemData<NestedJSONItemData, int>
    {
        readonly string m_Text;
        readonly string m_Template;
        readonly int m_Index;

        public string text { get { return m_Text; } }
        public override string template { get { return m_Template; } }
        public override int index { get { return m_Index; } }

        public NestedJSONItemData(JSONObject obj, string template, ref int index)
        {
            obj.GetField(ref m_Text, "text");
            m_Template = template;
            m_Index = index;
            var indexClosure = index + 1;
            obj.GetField("children", delegate(JSONObject childrenJSON)
            {
                var count = childrenJSON.Count;
                children = new List<NestedJSONItemData>(count);
                for (var i = 0; i < count; i++)
                {
                    children.Add(new NestedJSONItemData(childrenJSON[i], this.template, ref indexClosure));
                }
            });

            index = indexClosure;
        }
    }
}
