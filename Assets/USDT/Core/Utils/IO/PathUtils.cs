using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Utils {

    public static class PathUtils{

        public static string GetAssetsPath(string absPath) {
            absPath = absPath.Replace(@"\", "/");
            var relativePath = absPath.Substring(absPath.IndexOf(@"Assets/"));
            return relativePath;
        }

        public static string GetPlatformForAssetBundle() {
            switch (Application.platform) {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                default:
                    return string.Empty;
            }
        }

        public static string GetDataPath(params string[] folders) {
            string path = Application.dataPath;
            string subPath = string.Empty;
            for (int i = 0; i < folders.Length; i++) {
                if (i == folders.Length - 1) {
                    subPath = $"{subPath}{folders[i]}";
                }
                else {
                    subPath = $"{subPath}{folders[i]}/";
                }
                if (!string.IsNullOrEmpty(subPath)) {
                    DirectoryUtils.CreateDirectory($"{path}/{subPath}");
                }
            }
            return $"{path}/{subPath}";
        }
    }
}