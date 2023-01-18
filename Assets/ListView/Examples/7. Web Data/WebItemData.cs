using System.Data;

namespace Unity.Labs.ListView
{
    sealed class WebItemData : IListViewItemData<string>
    {
        readonly string m_Template;
        readonly string m_Index;
        readonly string m_Text;

        public string template
        {
            get { return m_Template; }
        }

        public string index
        {
            get { return m_Index; }
        }

        public string text
        {
            get { return m_Text; }
        }

        public WebItemData(JSONObject obj, string template)
        {
            m_Template = template;
            obj.GetField(ref m_Text, "name");
            var id = 0;
            if (!obj.GetField(ref id, "id"))
            {
                throw new MissingPrimaryKeyException(
                    string.Format("Web item data needs an index. Data does not contain an id field:\n{0}", obj));
            }

            m_Index = id.ToString();
        }
    }
}
