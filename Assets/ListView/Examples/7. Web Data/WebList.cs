using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Labs.ListView
{
    sealed class WebList : ListViewController<WebItemData, WebItem, string>
    {
        //Ideas for a better/different example web service are welcome
        //Note: the github API has a rate limit. After a couple of tries, you won't see any results :(

        [SerializeField]
        string m_URLFormatString = "https://api.github.com/gists/public?page={0}&per_page={1}";

        [SerializeField]
        string m_DefaultTemplate = "JSONItem";

        [SerializeField]
        int m_BatchSize = 15;

        [SerializeField]
        float m_Range;

        delegate void WebResult(List<WebItemData> data);

        int m_BatchOffset;
        bool m_WebLock;
        bool m_Loading;
        bool m_Cleanup;

        protected override float listHeight { get { return Mathf.Infinity; } }

        protected override void Awake()
        {
            base.Awake();
            size = Vector3.forward * m_Range;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(GetBatch(0, m_BatchSize * 3, webData => { data = webData; }));
        }

        IEnumerator GetBatch(int offset, int range, WebResult result)
        {
            if (m_WebLock)
                yield break;

            m_WebLock = true;

            var items = new List<WebItemData>(range);
            using (var www = new UnityWebRequest(string.Format(m_URLFormatString, offset, range)))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var response = new JSONObject(www.downloadHandler.text);
                    var count = response.list.Count;
                    for (var i = 0; i < count; i++)
                    {
                        items.Add(new WebItemData(response[i], m_DefaultTemplate));
                    }

                    result(items);
                }

                m_WebLock = false;
                m_Loading = false;
            }
        }

        protected override void ComputeConditions()
        {
            base.ComputeConditions();
            m_StartPosition = m_Extents - m_ItemSize * 0.5f;

            var dataOffset = (int)(-scrollOffset / m_ItemSize.z);

            var currentBatch = dataOffset / m_BatchSize;
            if (dataOffset > (m_BatchOffset + 2) * m_BatchSize)
            {
                //Check how many batches we jumped
                if (currentBatch == m_BatchOffset + 2) // Just one batch, fetch only the previous one
                {
                    StartCoroutine(GetBatch((m_BatchOffset + 3) * m_BatchSize, m_BatchSize, words =>
                    {
                        data.RemoveRange(0, m_BatchSize);
                        data.AddRange(words);
                        m_BatchOffset++;
                    }));
                }
                else if (currentBatch != m_BatchOffset) // Jumped multiple batches. Get a whole new data set
                {
                    if (!m_Loading)
                        m_Cleanup = true;

                    m_Loading = true;
                    StartCoroutine(GetBatch((currentBatch - 1) * m_BatchSize, m_BatchSize * 3, words =>
                    {
                        data = words;
                        m_BatchOffset = currentBatch - 1;
                    }));
                }
            }
            else if (m_BatchOffset > 0 && dataOffset < (m_BatchOffset + 1) * m_BatchSize)
            {
                if (currentBatch == m_BatchOffset) // Just one batch, fetch only the next one
                {
                    StartCoroutine(GetBatch((m_BatchOffset - 1) * m_BatchSize, m_BatchSize, words =>
                    {
                        data.RemoveRange(m_BatchSize * 2, m_BatchSize);
                        words.AddRange(data);
                        m_Data = words;
                        m_BatchOffset--;
                    }));
                }
                else if (currentBatch != m_BatchOffset) // Jumped multiple batches. Get a whole new data set
                {
                    if (!m_Loading)
                        m_Cleanup = true;

                    m_Loading = true;
                    if (currentBatch < 1)
                        currentBatch = 1;

                    StartCoroutine(GetBatch((currentBatch - 1) * m_BatchSize, m_BatchSize * 3, words =>
                    {
                        data = words;
                        m_BatchOffset = currentBatch - 1;
                    }));
                }
            }

            if (m_Cleanup)
            {
                foreach (var kvp in m_ListItems)
                {
                    var item = kvp.Value;
                    RecycleItem(item.data.template, item);
                }

                m_Cleanup = false;
            }
        }

        protected override void UpdateItems()
        {
            if (m_Data == null)
                return;

            var doneSettling = true;

            var itemWidth = m_ItemSize.z;
            var listWidth = m_Size.z;
            var offset = m_BatchOffset * m_BatchSize * itemWidth;
            var order = 0;
            var count = m_Data.Count;
            for (var i = 0; i < count; i++)
            {
                var datum = m_Data[i];
                var localOffset = offset + scrollOffset;
                if (localOffset + itemWidth < 0 || localOffset > listWidth)
                    Recycle(datum.index);
                else
                    UpdateVisibleItem(datum, order++, localOffset, ref doneSettling);

                offset += itemWidth;
            }

            if (m_Settling && doneSettling)
                EndSettling();
        }
    }
}
