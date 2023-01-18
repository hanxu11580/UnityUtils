namespace Unity.Labs.ListView
{
    sealed class JSONItemData : IListViewItemData<int>
    {
        readonly string m_Template;
        readonly string m_Text;
        readonly int m_Index;

        public int index
        {
            get { return m_Index; }
        }

        public string template
        {
            get { return m_Template; }
        }

        public string text
        {
            get { return m_Text; }
        }

        public JSONItemData(JSONObject obj, int index, string template)
        {
            m_Index = index;
            m_Template = template;
            obj.GetField(ref m_Text, "text");
        }
    }
}
