using System;
using System.Collections.Generic;
using System.IO;

namespace USDT.Utils {
    public static class DirectoryUtils
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static DirectoryInfo CreateDirectory(string path)
        {
            if (Directory.Exists(path)) return null;
            return Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 自定义删除文件夹（含删除内部文件及文件夹）
        /// 在删除大量的文件时，由于内部句柄没有被清理，产生The directory is not empty.异常
        /// 使用该方法，减少异常出现的几率，但不能完全避免， 成功率(未测试)几乎100%
        /// 具体信息参见https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true?answertab=votes#tab-top
        /// </summary>
        public static void DeleteDirectory(string path) {
            foreach (string directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }
            try {
                Directory.Delete(path, true);
            }
            catch (IOException) {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException) {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// 子文件夹是否存在
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static bool SubDirectoryExist(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists) return false;
            return directoryInfo.GetDirectories() != null;
        }
        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        /// <param name="path"></param>
        public static string GetDirectoryPath(string path)
        {
            string[] folders = path.Split('/');
            path = folders[0];
            CreateDirectory(path);
            string subPath = string.Empty;
            for (int i = 1; i < folders.Length; i++)
            {
                if (string.IsNullOrEmpty(folders[i])) continue;
                subPath = $"{subPath}{folders[i]}";
                CreateDirectory($"{path}/{subPath}");
                if (i == folders.Length - 1) continue;
                subPath = $"{subPath}/";
            }
            return $"{path}/{subPath}";
        }
        /// <summary>
        /// 获取子文件夹
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="directoryInfoList"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetDirectorys(DirectoryInfo directoryInfo, List<DirectoryInfo> directoryInfoList)
        {
            if (directoryInfo == null) return null;
            if (SubDirectoryExist(directoryInfo))
            {
                directoryInfoList.AddRange(directoryInfo.GetDirectories());
            }
            foreach (DirectoryInfo item in directoryInfo.GetDirectories())
            {
                GetDirectorys(item, directoryInfoList);
            }
            return directoryInfoList.ToArray();
        }

        /// <summary>
        /// 拷贝目录，包括子目录
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destDirectory"></param>
        public static void CopyDirectory(string sourceDirectory, string destDirectory) {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory)) {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory)) {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            CopyDirectoryFiles(sourceDirectory, destDirectory);
            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);
            foreach (string directionPath in directionName) {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = Path.Combine(destDirectory, directionPath.Substring(sourceDirectory.Length + 1)); // destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);
                                                                                                                             //递归下去
                CopyDirectory(directionPath, directionPathTemp);
            }
        }

        /// <summary>
        /// 只拷贝目录内文件，子目录不拷贝
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destDirectory"></param>
        public static void CopyDirectoryFiles(string sourceDirectory, string destDirectory) {
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

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="dir"></param>
        public static void GetAllFiles(List<string> files, string dir, bool includeSubDir) {
            string[] fls = Directory.GetFiles(dir);
            foreach (string fl in fls) {
                files.Add(fl);
            }

            if (includeSubDir) {
                string[] subDirs = Directory.GetDirectories(dir);
                foreach (string subDir in subDirs) {
                    GetAllFiles(files, subDir, includeSubDir);
                }
            }
        }
    }
}