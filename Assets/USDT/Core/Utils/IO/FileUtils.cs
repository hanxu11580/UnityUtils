﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace USDT.Utils {
    public static class FileUtils {

        /// <summary>
        /// Assets前一层路径
        /// </summary>
        /// <returns></returns>
        public static string UnityProjectPath() {
            var dirInfo = new DirectoryInfo(Application.dataPath);
            return dirInfo.Parent.FullName;
        }

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
        /// 获取文件夹大小
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static long GetDirectorySizeInBytes(string directoryPath) {
            if (!Directory.Exists(directoryPath)) {
                throw new DirectoryNotFoundException($"{directoryPath} couldn't find");
            }
            var directory = new DirectoryInfo(directoryPath);
            return directory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
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
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.SetLength(0);
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
            byte[] bytes = Encoding.UTF8.GetBytes(value);
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
        public static byte[] GetAsset(string path) {
            if (File.Exists(path)) {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
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
        public static byte[] GetAsset(string path, string name) {
            return GetAsset($"{path}/{name}");
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
                    lg.i("path dont exists ! : " + path);
                    return "";
                }

                StreamReader sr = File.OpenText(path);
                line.Append(sr.ReadToEnd());

                sr.Close();
                sr.Dispose();
            }
            catch (Exception e) {
                lg.e("Load text fail ! message:" + e.Message);
            }

            return line.ToString();
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

        #region 将File路径转成URI

        public static string FilePathToURI(string filePath) {
            var uriBuilder = new StringBuilder(filePath);
            for (int i = 0; i < uriBuilder.Length; i++) {
                char c = uriBuilder[i];

                if (AsciiCharactersAllowedInURIWithoutEscaping.Contains(c)) // This will be the vast majority of characters, so checking this first is best for performance
                    continue;

                if (c > '\xFF') // Not an ascii character
                    continue;

                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) {
                    uriBuilder[i] = '/';
                    continue;
                }


                int charAsInt = (int)c;
                string escapedCharacter = charAsInt.ToString("X2");

                uriBuilder[i] = '%';
                uriBuilder.Insert(i + 1, escapedCharacter);

                i += escapedCharacter.Length - 1;
            }


            if (uriBuilder.Length >= 2 && uriBuilder[0] == '/' && uriBuilder[1] == '/') // UNC path
                uriBuilder.Insert(0, "file:");
            else
                uriBuilder.Insert(0, "file:///");

            return uriBuilder.ToString();
        }

        static readonly HashSet<char> AsciiCharactersAllowedInURIWithoutEscaping = GetAsciiCharactersAllowedInURI();
        static HashSet<char> GetAsciiCharactersAllowedInURI() {
            var set = new HashSet<char>();

            for (int i = 'a'; i <= 'z'; i++)
                set.Add((char)i);

            for (int i = 'A'; i <= 'Z'; i++)
                set.Add((char)i);

            for (int i = '0'; i <= '9'; i++)
                set.Add((char)i);

            set.Add('+');
            set.Add('/');
            set.Add(':');
            set.Add('.');
            set.Add('-');
            set.Add('_');
            set.Add('~');

            return set;
        }

        #endregion
    }
}