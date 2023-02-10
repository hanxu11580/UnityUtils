using System.Collections.Generic;
using System.IO;

namespace Utility {
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

        public static void CopyDirectory(string sourceDirectory, string destDirectory) {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory)) {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory)) {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            FileUtils.CopyFile(sourceDirectory, destDirectory);
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
    }
}