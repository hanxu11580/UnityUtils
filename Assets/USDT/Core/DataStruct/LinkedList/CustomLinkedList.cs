using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace USDT.Core {
    /// <summary>
    /// 本质就是对LinkedList的封装
    /// 添加了复用Node，多了点功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomLinkedList<T> : ICollection<T>, IEnumerable<T>
    {
        private readonly LinkedList<T> m_LinkedList;

        //就是一个对象池 value:LinkedListNode
        private Queue<LinkedListNode<T>> m_CachedNodes;

        public CustomLinkedList()
        {
            m_LinkedList = new LinkedList<T>();
            m_CachedNodes = new Queue<LinkedListNode<T>>();
        }

        /// <summary>
        /// 链表节点数量
        /// </summary>
        public int Count => m_LinkedList.Count;

        /// <summary>
        /// 缓存节点数量
        /// </summary>
        public int CachedNodeCount => m_CachedNodes.Count;

        /// <summary>
        /// 链表第一个节点
        /// </summary>
        public LinkedListNode<T> First => m_LinkedList.First;

        /// <summary>
        /// 链表最后一个节点
        /// </summary>
        public LinkedListNode<T> Last => m_LinkedList.Last;

        /// <summary>
        /// 为true是将无法add或remove(没细了解)
        /// </summary>
        public bool IsReadOnly => ((ICollection<T>)m_LinkedList).IsReadOnly;


        #region 基础功能

        /// <summary>
        /// 向链表某节点屁股加一个新节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> afterNode = AcquireNode(value);
            m_LinkedList.AddAfter(node, afterNode);
            return afterNode;
        }

        /// <summary>
        /// 向链表某节点前面加一个新节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> beforeNode = AcquireNode(value);
            m_LinkedList.AddBefore(node, beforeNode);
            return beforeNode;
        }

        /// <summary>
        /// 向链表头部添加一个节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> firstNode = AcquireNode(value);
            m_LinkedList.AddFirst(firstNode);
            return firstNode;
        }

        /// <summary>
        /// 向链表最后添加一个节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> lastNode = AcquireNode(value);
            m_LinkedList.AddLast(lastNode);
            return lastNode;
        }

        public void ClearCachedNode()
        {
            m_CachedNodes.Clear();
        }

        /// <summary>
        /// 查找指定值，返回第一找到的
        /// </summary>
        /// <param name="value"></param>
        public LinkedListNode<T> Find(T value)
        {
            return m_LinkedList.Find(value);
        }

        /// <summary>
        /// 查找指定值，返回最后一个找到的
        /// </summary>
        /// <param name="value"></param>
        public void FindLast(T value)
        {
            m_LinkedList.FindLast(value);
        }

        public void Clear()
        {
            LinkedListNode<T> curr = m_LinkedList.First;
            while(curr != null)
            {
                ReleaseNode(curr);
                curr = curr.Next;
            }
            //为什么不直接clear，因为要复用Node
            m_LinkedList.Clear();
        }

        public bool Contains(T value)
        {
            return m_LinkedList.Contains(value);
        }

        /// <summary>
        /// 浅拷贝 拷贝至已有数组中,将整个链表
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection)m_LinkedList).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <returns>是否移除成功,false:没有这个节点</returns>
        public bool Remove(T value)
        {
            LinkedListNode<T> node = m_LinkedList.Find(value);
            if(node != null)
            {
                m_LinkedList.Remove(value);
                ReleaseNode(node);
                return true;
            }
            return false;
        }

        public void Remove(LinkedListNode<T> node)
        {
            m_LinkedList.Remove(node);
            ReleaseNode(node);
        }

        /// <summary>
        /// 移除位于链表开头处的结点。
        /// </summary>
        public void RemoveFirst()
        {
            LinkedListNode<T> first = m_LinkedList.First;
            if (first == null)
            {
                Debug.LogError("First is invalid.");
            }
            m_LinkedList.RemoveFirst();
            ReleaseNode(first);
        }

        /// <summary>
        /// 移除位于链表结尾处的结点。
        /// </summary>
        public void RemoveLast()
        {
            LinkedListNode<T> last = m_LinkedList.Last;
            if (last == null)
            {
                Debug.LogError("Last is invalid.");
            }

            m_LinkedList.RemoveLast();
            ReleaseNode(last);
        }

        public void Add(T value)
        {
            AddLast(value);
        }

        #endregion

        #region Private
        LinkedListNode<T> AcquireNode(T value)
        {
            LinkedListNode<T> node;
            if (CachedNodeCount > 0)
            {
                node = m_CachedNodes.Dequeue();
                node.Value = value; //这一步很重要，容易忘
            }
            else
            {
                node = new LinkedListNode<T>(value);
            }
            return node;
        }

        void ReleaseNode(LinkedListNode<T> node)
        {
            node.Value = default;
            m_CachedNodes.Enqueue(node);
        }

        #endregion


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(m_LinkedList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(m_LinkedList);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 循环访问集合的枚举数。
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>
        {
            private LinkedList<T>.Enumerator m_Enumerator;

            internal Enumerator(LinkedList<T> linkedList)
            {
                if (linkedList == null)
                {
                    Debug.LogError("Linked list is invalid.");
                }

                m_Enumerator = linkedList.GetEnumerator();
            }

            /// <summary>
            /// 获取当前结点。
            /// </summary>
            public T Current => m_Enumerator.Current;

            /// <summary>
            /// 获取当前的枚举数。
            /// </summary>
            object IEnumerator.Current => m_Enumerator.Current;

            /// <summary>
            /// 清理枚举数。
            /// </summary>
            public void Dispose()
            {
                m_Enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个结点。
            /// </summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext()
            {
                //正常需要写取下一个的逻辑
                //需要什么下标什么的，内置LinkedList包含了所以直接用
                return m_Enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数。
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<T>)m_Enumerator).Reset();
            }
        }
    }
}
