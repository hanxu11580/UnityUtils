using System.Collections.Generic;
using UnityEngine;

namespace USDT.Expand {
    public static class ListExpand
    {
        /// <summary>
        /// 高效删除List数据 防止移除后 向间隙移动
        /// 但会破坏顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls"></param>
        /// <param name="index"></param>
        public static void ExRemoveAt<T>(this List<T> ls, int index)
        {
            if (index >= ls.Count) Debug.LogError("Index超过List索引");
            int lastIndex = ls.Count - 1;
            ls[index] = ls[lastIndex];
            ls.RemoveAt(lastIndex);
        }
        public static void ExRemove<T>(this List<T> ls, T data)
        {
            if(!ls.Contains(data)) Debug.LogError("移除数据不存在List中");
            int index = ls.IndexOf(data);
            ls.ExRemoveAt(index);
        }
    }
}
