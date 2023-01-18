using System.Collections.Generic;
using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class AdvancedList : NestedListViewController<AdvancedListItemData, AdvancedListItem, int>
    {
        sealed class ModelPool
        {
            public readonly GameObject prefab;
            public readonly Queue<Transform> pool = new Queue<Transform>();

            public ModelPool(GameObject prefab)
            {
                if (prefab == null)
                    Debug.LogError("Template prefab cannot be null");

                this.prefab = prefab;
            }
        }

        [SerializeField]
        string m_DataFile;

        [SerializeField]
        GameObject[] m_Models;

        [SerializeField]
        float m_Range;

        readonly Dictionary<string, ModelPool> m_ModelDictionary = new Dictionary<string, ModelPool>();
        readonly Dictionary<string, Vector3> m_TemplateSizes = new Dictionary<string, Vector3>();

        protected override void Awake()
        {
            base.Awake();
            size = m_Range * Vector3.forward;
        }

        protected override void Start()
        {
            base.Start();

            foreach (var kvp in m_TemplateDictionary)
            {
                m_TemplateSizes[kvp.Key] = GetObjectSize(kvp.Value.prefab);
            }

            var text = Resources.Load<TextAsset>(m_DataFile);
            if (text)
            {
                var obj = new JSONObject(text.text);
                var length = obj.Count;
                data = new List<AdvancedListItemData>(length);
                var index = 0;
                for (var i = 0; i < length; i++)
                {
                    var item = new AdvancedListItemData(obj[i], ref index);
                    data.Add(item);
                }
            }
            else
            {
                data = null;
            }

            if (m_Models.Length < 1)
                Debug.LogError("No models!");

            foreach (var model in m_Models)
            {
                var modelName = model.name;
                if (m_ModelDictionary.ContainsKey(modelName))
                    Debug.LogError("Two templates cannot have the same name");

                m_ModelDictionary[modelName] = new ModelPool(model);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(m_ItemSize.x, m_Size.y, m_ItemSize.z));
        }

        protected override void ComputeConditions()
        {
            base.ComputeConditions();
            m_StartPosition = m_Extents - m_ItemSize * 0.5f;
        }

        protected override void UpdateNestedItems(ref int order, ref float offset, ref bool doneSettling, int depth = 0)
        {
            m_UpdateStack.Push(new UpdateData
            {
                data = m_Data,
                depth = depth
            });

            var listWidth = m_Size.z;
            while (m_UpdateStack.Count > 0)
            {
                var stackData = m_UpdateStack.Pop();
                var nestedData = stackData.data;
                depth = stackData.depth;

                var i = stackData.index;
                for (; i < nestedData.Count; i++)
                {
                    var datum = nestedData[i];

                    var index = datum.index;
                    bool expanded;
                    if (!m_ExpandStates.TryGetValue(index, out expanded))
                        m_ExpandStates[index] = false;

                    var itemWidth = m_TemplateSizes[datum.template].z;
                    var localOffset = offset + scrollOffset;
                    if (localOffset + itemWidth < 0 || localOffset > listWidth)
                        Recycle(index);
                    else
                        UpdateNestedItem(datum, order++, localOffset, depth, ref doneSettling);

                    offset += itemWidth;

                    if (datum.children != null)
                    {
                        if (expanded)
                        {
                            m_UpdateStack.Push(new UpdateData
                            {
                                data = nestedData,
                                depth = depth,
                                index = i + 1
                            });

                            m_UpdateStack.Push(new UpdateData
                            {
                                data = datum.children,
                                depth = depth + 1
                            });
                            break;
                        }

                        RecycleChildren(datum);
                    }
                }
            }
        }

        protected override void RecycleItem(string template, AdvancedListItem item)
        {
            base.RecycleItem(template, item);

            var itemChild = item as AdvancedListItemChild;
            if (itemChild == null)
                return;

            var model = itemChild.model;
            m_ModelDictionary[itemChild.data.model].pool.Enqueue(model);
            model.parent = transform;
            model.gameObject.SetActive(false);
        }

        protected override bool GetNewItem(AdvancedListItemData datum, out AdvancedListItem item)
        {
            var instantiated = base.GetNewItem(datum, out item);
            var itemChild = item as AdvancedListItemChild;
            if (itemChild != null)
                itemChild.SetModel(GetModel(datum.model));

            return instantiated;
        }

        Transform GetModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                return null;

            ModelPool modelPool;
            if (!m_ModelDictionary.TryGetValue(modelName, out modelPool))
            {
                Debug.LogWarning(string.Format("Cannot get model, {0} doesn't exist", modelName));
                return null;
            }

            var pool = modelPool.pool;
            Transform model;
            if (pool.Count > 0)
            {
                model = pool.Dequeue();
                model.gameObject.SetActive(true);
            }
            else
            {
                model = Instantiate(modelPool.prefab).transform;
                model.parent = transform;
            }

            return model;
        }
    }
}
