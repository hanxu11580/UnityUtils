using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

//Borrows from http://answers.unity3d.com/questions/743400/database-sqlite-setup-for-unity.html
//Dictionary from https://wordnet.princeton.edu/

namespace Unity.Labs.ListView
{
    sealed class DictionaryList : ListViewController<DictionaryListItemData, DictionaryListItem, KeyValuePair<int, int>>
    {
        [SerializeField]
        int m_BatchSize = 15;

        [SerializeField]
        string m_DefaultTemplate = "DictionaryItem";

        [SerializeField]
        GameObject m_LoadingIndicator;

        [SerializeField]
        int m_MaxWordCharacters = 30;

        [SerializeField]
        int m_DefinitionCharacterWrap = 40;

        [SerializeField]
        int m_MaxDefinitionLines = 4;

        [SerializeField]
        float m_Range = 12f;

        delegate void WordsResult(List<DictionaryListItemData> words);

        protected override float listHeight { get { return m_DataLength * m_ItemSize.z; } }

        volatile bool m_DBLock;

        bool m_Cleanup;
        int m_DataLength; //Total number of items in the data set
        int m_BatchOffset; //Number of batches we are offset
        bool m_Loading;

        IDbConnection m_DBConnection;

        readonly StringBuilder m_StringBuilder = new StringBuilder();

        protected override void Awake()
        {
            base.Awake();
            size = Vector3.forward * m_Range;
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            var conn = string.Format("URI=file:{0}", Path.Combine(Application.dataPath, DictionaryResourceStrings.editorDatabasePath));
#else
            var conn = string.Format("URI=file:{0}", Path.Combine(Application.dataPath, DictionaryResourceStrings.databasePath));
#endif

            m_DBConnection = new SqliteConnection(conn);
            m_DBConnection.Open(); //Open connection to the database.

            if (m_MaxWordCharacters < 4)
                Debug.LogError("Max word length must be > 3");

            try
            {
                var dbCommand = m_DBConnection.CreateCommand();
                const string sqlQuery = "SELECT COUNT(lemma) FROM word as W JOIN sense as S on W.wordid=S.wordid JOIN synset as Y on S.synsetid=Y.synsetid";
                dbCommand.CommandText = sqlQuery;
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    m_DataLength = reader.GetInt32(0);
                }

                reader.Close();
                dbCommand.Dispose();
            }
            catch
            {
                Debug.LogError("DB error, couldn't get total data length");
            }

            //Start off with some data
            var range = m_BatchSize * 3;
            m_Data = new List<DictionaryListItemData>(range);
            GetWords(0, range, words => { m_Data.AddRange(words); });
        }

        void OnDestroy()
        {
            m_DBConnection.Close();
            m_DBConnection = null;
        }

        void GetWords(int offset, int range, WordsResult result)
        {
            Debug.Assert(result != null, "Called GetWords without a result callback");

            if (m_DBLock)
                return;

            m_DBLock = true;
            new Thread(() =>
            {
                try
                {
                    var words = new List<DictionaryListItemData>(range);
                    var dbCommand = m_DBConnection.CreateCommand();
                    var sqlQuery = string.Format("SELECT W.wordid, Y.synsetid, lemma, definition FROM word as W JOIN sense as S on W.wordid=S.wordid JOIN synset as Y on S.synsetid=Y.synsetid ORDER BY W.wordid limit {0} OFFSET {1}", range, offset);
                    dbCommand.CommandText = sqlQuery;
                    var reader = dbCommand.ExecuteReader();
                    var count = 0;
                    while (reader.Read())
                    {
                        var wordid = reader.GetInt32(0);
                        var synsetid = reader.GetInt32(1);
                        var id = new KeyValuePair<int, int>(wordid, synsetid);
                        var lemma = reader.GetString(2);
                        var definition = reader.GetString(3);

                        //truncate word if necessary
                        if (lemma.Length > m_MaxWordCharacters)
                            lemma = lemma.Substring(0, m_MaxWordCharacters - 3) + "...";

                        //Wrap definition
                        var definitionLines = definition.Split(' ');
                        var charCount = 0;
                        var lineCount = 0;
                        m_StringBuilder.Length = 0;
                        foreach (var line in definitionLines)
                        {
                            charCount += line.Length + 1;
                            if (charCount > m_DefinitionCharacterWrap)
                            {
                                if (++lineCount >= m_MaxDefinitionLines)
                                {
                                    m_StringBuilder.Append("...");
                                    break;
                                }

                                m_StringBuilder.Append("\n");
                                charCount = 0;
                            }

                            m_StringBuilder.Append(line);
                            m_StringBuilder.Append(" ");
                        }

                        words.Add(new DictionaryListItemData(lemma, m_StringBuilder.ToString(), id, m_DefaultTemplate));

                        count++;
                    }

                    if (count < m_BatchSize)
                        Debug.LogWarning("reached end");

                    reader.Close();
                    dbCommand.Dispose();
                    result(words);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception reading from DB: " + e.Message);
                }

                m_DBLock = false;
                m_Loading = false;
            }).Start();
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
                    GetWords((m_BatchOffset + 3) * m_BatchSize, m_BatchSize, words =>
                    {
                        m_Data.RemoveRange(0, m_BatchSize);
                        m_Data.AddRange(words);
                        m_BatchOffset++;
                    });
                }
                else if (currentBatch != m_BatchOffset) // Jumped multiple batches. Get a whole new data set
                {
                    if (!m_Loading)
                        m_Cleanup = true;

                    m_Loading = true;
                    GetWords((currentBatch - 1) * m_BatchSize, m_BatchSize * 3, words =>
                    {
                        m_Data = words;
                        m_BatchOffset = currentBatch - 1;
                    });
                }
            }
            else if (m_BatchOffset > 0 && dataOffset < (m_BatchOffset + 1) * m_BatchSize)
            {
                if (currentBatch == m_BatchOffset) // Just one batch, fetch only the next one
                {
                    GetWords((m_BatchOffset - 1) * m_BatchSize, m_BatchSize, words =>
                    {
                        m_Data.RemoveRange(m_BatchSize * 2, m_BatchSize);
                        words.AddRange(m_Data);
                        m_Data = words;
                        m_BatchOffset--;
                    });
                }
                else if (currentBatch != m_BatchOffset) // Jumped multiple batches. Get a whole new data set
                {
                    if (!m_Loading)
                        m_Cleanup = true;

                    m_Loading = true;
                    if (currentBatch < 1)
                        currentBatch = 1;

                    GetWords((currentBatch - 1) * m_BatchSize, m_BatchSize * 3, words =>
                    {
                        m_Data = words;
                        m_BatchOffset = currentBatch - 1;
                    });
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
            var count = m_Data.Count;
            if (count == 0 || m_Loading)
            {
                m_LoadingIndicator.SetActive(true);
                return;
            }

            var doneSettling = true;
            var itemWidth = m_ItemSize.z;
            var listWidth = m_Size.z;
            var offset = m_BatchOffset * m_BatchSize * itemWidth;
            var order = 0;
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

            m_LoadingIndicator.SetActive(false);
        }
    }
}
