using System.Collections.Generic;
using USDT.Utils;

namespace USDT.Extensions {
    public static class ListExtension
    {
        /// <summary>
        /// 高效删除List数据 防止移除后 向间隙移动
        /// 但会破坏顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ls"></param>
        /// <param name="index"></param>
        public static void FastRemoveAt<T>(this List<T> ls, int index)
        {
            if (index >= ls.Count) return;
            int lastIndex = ls.Count - 1;
            ls[index] = ls[lastIndex];
            ls.RemoveAt(lastIndex);
        }
        public static void FastRemove<T>(this List<T> ls, T data)
        {
            if(!ls.Contains(data)) return;
            int index = ls.IndexOf(data);
            ls.FastRemoveAt(index);
        }

        public static void Log<T>(this List<T> ls) {
            if(ls == null) {
                lg.i($"log list is null");
                return;
            }
            foreach (var item in ls) {
                lg.i(item.ToString());
            }
        }
    }
}
