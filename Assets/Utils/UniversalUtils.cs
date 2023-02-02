using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utility {
    public static class UniversalUtils {

        public static object Deserialize(byte[] bytes) {
            object obj = null;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream(bytes)) {
                    obj = formatter.Deserialize(mStream);
                }
            }
            catch (Exception e) {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return obj;
        }

        public static byte[] Serialize(object obj) {
            byte[] ret = default;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream()) {
                    formatter.Serialize(mStream, obj);
                    ret = mStream.GetBuffer();
                }
            }
            catch (Exception e) {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return ret;
        }

        public static T DeepCopyWithBinarySerialize<T>(T obj) {
            object retval;
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter bf = new BinaryFormatter();
                // 序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                // 反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }

            return (T)retval;
        }

        public static string[] GetLowLetter() {
            var letterArr = new string[26];
            for (char i = 'a'; i <= 'z'; i++) {
                var charInt = (int)i;
                letterArr[charInt - 97] = i.ToString();
            }

            return letterArr;
        }
    }
}
