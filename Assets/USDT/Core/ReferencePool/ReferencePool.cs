using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace USDT.Core {
    /// <summary>
    /// 引用池
    /// </summary>
    public static class ReferencePool
    {
        /// <summary>
        /// 引用集合字典 key:类型 每个类型对应自己得集合
        /// </summary>
        private static Dictionary<string, ReferenceCollection> _ReferenceCollections =
            new Dictionary<string, ReferenceCollection>();

        /// <summary>
        /// 引用池数量
        /// </summary>
        public static int Count => _ReferenceCollections.Count;

        /// <summary>
        /// 获取引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Acquire<T>()where T:class,IReference,new()
        {
            return GetReferenceCollection(typeof(T).FullName).Acquire<T>();
        }

        /// <summary>
        /// 回收引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refe"></param>
        public static void Release<T>(T refe) where T : class, IReference
        {
            if(refe == null)
            {
                Debug.LogError("要归还的引用为空");
            }
            GetReferenceCollection(typeof(T).FullName).Release<T>(refe);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void ClearAll()
        {
            foreach (ReferenceCollection referenceCollection in _ReferenceCollections.Values)
            {
                referenceCollection.Clear();
            }
            _ReferenceCollections.Clear();
        }

        /// <summary>
        /// 移除对应ReferenceCollection所有引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RemoveAll<T>()where T:class, IReference
        {
            GetReferenceCollection(typeof(T).FullName).Clear();
        }

        //--Private

        /// <summary>
        /// 获取ReferenceCollection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static ReferenceCollection GetReferenceCollection(string name)
        {
            if (!_ReferenceCollections.TryGetValue(name, out ReferenceCollection referenceCollection))
            {
                referenceCollection = new ReferenceCollection();
                _ReferenceCollections.Add(name, referenceCollection);
            }
            return referenceCollection;
        }



    }
}
