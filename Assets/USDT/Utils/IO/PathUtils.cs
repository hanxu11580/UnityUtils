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

    }
}