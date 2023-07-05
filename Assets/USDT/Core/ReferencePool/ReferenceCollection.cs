using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace USDT.Core {
    /// <summary>
    /// 引用集合
    /// </summary>
    public sealed class ReferenceCollection
    {
        private Queue<IReference> _referenceQueue;
        public ReferenceCollection()
        {
            _referenceQueue = new Queue<IReference>();
        }

        /// <summary>引用数量</summary>
        public int RefCount => _referenceQueue.Count;

        /// <summary>生产引用实例</summary>
        public T Acquire<T>() where T : class, IReference, new()
        {
            T refe;
            if (_referenceQueue.Count > 0)
            {
                refe = _referenceQueue.Dequeue() as T;
            }
            else
            {
                refe = new T();
            }
            return refe;
        }

        /// <summary>生产引用实例</summary>
        public IReference Acquire(Type type)
        {
            IReference refe;
            if (_referenceQueue.Count > 0)
            {
                refe = _referenceQueue.Dequeue();
            }
            else
            {
                refe = Activator.CreateInstance(type) as IReference;
            }
            return refe;
        }

        /// <summary>回收用实例(回收前执行Reset)</summary>
        public void Release<T>(T refe)where T:class,IReference
        {
            refe.ClearRef();
            _referenceQueue.Enqueue(refe);
        }

        /// <summary>清空</summary>
        public void Clear()
        {
            _referenceQueue.Clear();
        }
    }
}

