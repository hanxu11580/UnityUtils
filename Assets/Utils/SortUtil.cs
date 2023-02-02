
using System;

namespace Utility {
    /// <summary>
    /// 常用排序
    /// </summary>
    public static class SortUtil
    {
        /// <summary>
        /// 快速排序(递归)QuickSort一层深度执行，将会使s的前面都小于s，s的后面都大于s
        /// 最后采用递归 达到分治效果
        /// 泛型T无法比较 所以使用限制 T 只有实现了IComparable才可以使用
        /// </summary>
        /// <param name="myArray"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void QuickSort<T>(T[] myArray, int lIndex, int rIndex) where T : IComparable<T>
        {
            int i, j;
            T s;
            if (lIndex < rIndex)
            {
                i = lIndex - 1;
                j = rIndex + 1;
                s = myArray[(i + j) / 2]; //取中间值 如果数据为偶数 则为中间2个的左边
                while (true)
                {
                    while (myArray[++i].CompareTo(s) < 0) ; //如果左边数小于s那么向右移动
                    while (myArray[--j].CompareTo(s) > 0) ;//如果左边数大于s那么向左移动
                    if (i >= j)
                        break;

                    //交换i和j的值
                    var temp = myArray[i];
                    myArray[i] = myArray[j];
                    myArray[j] = temp;
                }
                
                //采用分治手段
                QuickSort(myArray, lIndex, i - 1);
                QuickSort(myArray, j + 1, rIndex);
            }

        }
    }
}
