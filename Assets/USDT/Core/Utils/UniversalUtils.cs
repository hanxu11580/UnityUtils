using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;

namespace USDT.Utils {
    public static class UniversalUtils {

        #region Serialize/Deserialize
        /// <summary>
        /// 将对象序列化成字节数组
        /// </summary>
        public static byte[] SerializeToBinary<T>(T obj) {
            byte[] ret = default;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream()) {
                    formatter.Serialize(mStream, obj);
                    ret = mStream.GetBuffer();
                }
            }
            catch (Exception e) {
                LogUtils.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return ret;
        }

        /// <summary>
        /// 从字节数组反序列成对象
        /// </summary>
        public static T DeserializeFromBinary<T>(byte[] bytes) {
            T obj = default;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream(bytes)) {
                    obj = (T)formatter.Deserialize(mStream);
                }
            }
            catch (Exception e) {
                LogUtils.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return (T)obj;
        }

        /// <summary>
        /// 序列化到某文件中
        /// </summary>
        public static void SerializeToPath<T>(string path, T obj){
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// 从某文件反序列成对象
        /// </summary>
        public static T DeserializeFromPath<T>(string path){
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(fs);
            }
        }

        public static void LitJsonToJson<T>(string path, T obj) {
            if(obj == null) {
                LogUtils.LogError($"对象为空");
                return;
            }
            if (File.Exists(path)) {
                File.Delete(path);
                LogUtils.Log($"{path}存在，已删除");
            }
            var json = JsonMapper.ToJson(obj);
            File.WriteAllText(path, json);
        }

        public static T LitJsonToObject<T>(string path) {
            if (!File.Exists(path)) {
                LogUtils.Log($"{path}不存在");
                return default;
            }
            var json = File.ReadAllText(path);
            var obj = JsonMapper.ToObject<T>(json);
            return obj;
        }

        #endregion

        /// <summary>
        /// 深度拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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
    }
}
