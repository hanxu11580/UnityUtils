using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility {
    public static class FileUtils
    {
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static bool FileExist(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists) return false;
            return directoryInfo.GetFiles() != null;
        }
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static long GetFileSize(string path)
        {
            if (!File.Exists(path)) return -1;
            return new FileInfo(path).Length;
        }
        /// <summary>
        /// 获取所有子文件
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="fileInfoList"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(DirectoryInfo directoryInfo, List<FileInfo> fileInfoList)
        {
            if (directoryInfo == null) return null;
            if (FileExist(directoryInfo))
            {
                fileInfoList.AddRange(directoryInfo.GetFiles());
            }
            foreach (DirectoryInfo item in directoryInfo.GetDirectories())
            {
                GetFiles(item, fileInfoList);
            }
            return fileInfoList.ToArray();
        }
        /// <summary>
        /// 保存资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public static void SaveAsset(string path, byte[] bytes)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
        /// <summary>
        /// 保存资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public static void SaveAsset(string path, string value)
        {
            byte[] bytes = value.GetBytes();
            SaveAsset(path, bytes);
        }
        /// <summary>
        /// 保存资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="bytes"></param>
        public static void SaveAsset(string path, string name, byte[] bytes)
        {
            SaveAsset($"{path}/{name}", bytes);
        }
        /// <summary>
        /// 保存资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveAsset(string path, string name, string value)
        {
            SaveAsset($"{path}/{name}", value);
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string path, string name)
        {
            return GetBytes($"{path}/{name}");
        }

        public static void CopySingleFile(string sourceFilePath, string destFilePath) {
            string directoryName = Path.GetDirectoryName(destFilePath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
            //若不存在，直接复制文件；若存在，覆盖复制
            if (File.Exists(destFilePath)) {
                File.Copy(sourceFilePath, destFilePath, true);
            }
            else {
                File.Copy(sourceFilePath, destFilePath);
            }
        }

        public static string ReadStringByFile(string path) {
            StringBuilder line = new StringBuilder();
            try {
                if (!File.Exists(path)) {
                    UnityEngine.Debug.Log("path dont exists ! : " + path);
                    return "";
                }

                StreamReader sr = File.OpenText(path);
                line.Append(sr.ReadToEnd());

                sr.Close();
                sr.Dispose();
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("Load text fail ! message:" + e.Message);
            }

            return line.ToString();
        }


        public static void CopyFile(string sourceDirectory, string destDirectory) {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);
            foreach (string filePath in fileName) {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = Path.Combine(destDirectory, filePath.Substring(sourceDirectory.Length + 1)); // destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
                                                                                                                   //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp)) {
                    File.Copy(filePath, filePathTemp, true);
                }
                else {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }

        #region MD5
        public static string CalculateMD5(byte[] bytes) {
            string md5Str = null;
            if (null != bytes) {
                using (var md5 = System.Security.Cryptography.MD5.Create()) {
                    var hash = md5.ComputeHash(bytes);
                    md5Str = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            return md5Str;
        }

        public static string CalculateMD5(string filename) {
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                try {
                    using (var stream = File.OpenRead(filename)) {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
                catch (Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                }
                return null;
            }
        }
        #endregion
    }
}