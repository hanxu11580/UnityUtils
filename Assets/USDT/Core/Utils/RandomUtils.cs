using System;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

namespace USDT.Utils
{    
    public static class RandomUtils
    {
        public static Random random = new Random(Guid.NewGuid().GetHashCode());
       
        /// <summary>
        /// 获取lower与Upper之间的随机数,包含下限，不包含上限
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int RandomNumber(int lower, int upper)
        {
            int value = random.Next(lower, upper);
            return value;
        }

        public static bool RandomBool()
        {
            return random.Next(2) == 0;
        }

        public static T RandomArray<T>(T[] array)
        {
            return array[RandomNumber(0, array.Length)];
        }

        public static T RandomArray<T>(List<T> array)
        {
            return array[RandomNumber(0, array.Count)];
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要打乱的数组</param>
        public static void DisruptList<T>(List<T> arr)
        {
            if (arr == null || arr.Count < 2)
            {
                return;
            }

            for (int i = 0; i < arr.Count; i++)
            {
                int index = random.Next(0, arr.Count);
                T temp = arr[index];
                arr[index] = arr[i];
                arr[i] = temp;
            }
        }
        
        /// <summary>
        /// 随机从数组中取若干个不重复的元素，
        /// 为了降低算法复杂度，所以是伪随机，对随机要求不是非常高的逻辑可以用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceList"></param>
        /// <param name="destList"></param>
        /// <param name="randCount"></param>
        public static bool GetRandListByCount<T>(List<T> sourceList, List<T> destList, int randCount)
        {
            if (sourceList == null || destList == null || randCount < 0)
            {
                return false;
            }
            
            destList.Clear();

            if (randCount >= sourceList.Count)
            {
                foreach (var val in sourceList)
                {
                    destList.Add(val);
                }
                
                return true;
            }

            if (randCount == 0)
            {
                return true;
            }
            int beginIndex = random.Next(0, sourceList.Count - 1);
            for (int i = beginIndex; i < beginIndex + randCount; i++)
            {
                destList.Add(sourceList[i % sourceList.Count]);
            }

            return true;
        }

        /// <summary>
        /// 0~1
        /// </summary>
        /// <returns></returns>
        public static float RandFloat01()
        {
            int a = RandomNumber(0, 1000000);
            return a / 1000000f;
        }
    }
}