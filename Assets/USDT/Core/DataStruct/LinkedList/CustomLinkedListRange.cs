using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace USDT.Core {

    /// <summary>
    /// 链表范围(本质就是一段链表)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [StructLayout(LayoutKind.Auto)]
    public struct CustomLinkedListRange<T> : IEnumerable<T>
    {
        public LinkedListNode<T> First { get; }

        //不包含终点
        public LinkedListNode<T> Terminal { get; }

        public CustomLinkedListRange(LinkedListNode<T> first, LinkedListNode<T> terminal)
        {
            if (first == null || terminal == null || first == terminal)
            {
                Debug.LogError("first == Terminal");
            }

            First = first;
            Terminal = terminal;
        }

        /// <summary>
        /// 范围是否合法
        /// </summary>
        public bool IsValid => First != null && Terminal != null && First != Terminal;

        /// <summary>
        /// 范围内节点数量
        /// </summary>
        public int Count
        {
            get
            {
                if (!IsValid) return 0;
                int count = 0;
                for (LinkedListNode<T> curr = First;
                    curr != null && curr != Terminal;
                    curr = curr.Next)
                {
                    count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 范围内是否包含某值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            for (LinkedListNode<T> curr = First;
                curr != null && curr != Terminal;
                curr = curr.Next)
            {
                if (curr.Value.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator GetEnumerator()
        {
            //Error 根据构造函数，需要传入当前结构，这我忘传了
            return new Enumerator(this);
        }

        #region Enumerator

        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>
        {
            private readonly CustomLinkedListRange<T> _XhO_OKitLinkedListRange;
            private LinkedListNode<T> _curr;
            private T _currValue;

            public Enumerator(CustomLinkedListRange<T> XhO_OKitLinkedListRange)
            {
                if (!XhO_OKitLinkedListRange.IsValid)
                {
                    Debug.LogError("XhO_OKitLinkedListRange 不合法");
                }
                _XhO_OKitLinkedListRange = XhO_OKitLinkedListRange;
                _curr = _XhO_OKitLinkedListRange.First;
                _currValue = default;
            }

            public T Current => _currValue;

            object IEnumerator.Current => _currValue;

            public void Dispose() { }

            public bool MoveNext()
            {
                //下一个
                if (_curr == null || _curr == _XhO_OKitLinkedListRange.Terminal)
                {
                    return false;
                }
                _currValue = _curr.Value;
                _curr = _curr.Next;
                return true;


            }

            public void Reset()
            {
                _curr = _XhO_OKitLinkedListRange.First;
                _currValue = default;
            }
        }

        #endregion
    }
}
