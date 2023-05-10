using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace USDT.CustomEditor {
    public static partial class EditorUtils {

        public static List<GameObject> GetAllPrefabByAssetDatabase(params string[] path) {
            List<GameObject> _prefabList = new List<GameObject>();
            string[] _guids = AssetDatabase.FindAssets("t:Prefab", path);
            string _prefabPath = "";
            GameObject _prefab;
            foreach (var _guid in _guids) {
                _prefabPath = AssetDatabase.GUIDToAssetPath(_guid);
                _prefab = AssetDatabase.LoadAssetAtPath(_prefabPath, typeof(GameObject)) as GameObject;
                _prefabList.Add(_prefab);
            }
            return _prefabList;
        }

        public static List<GameObject> GetAllPrefabByDirectory(string path) {
            string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            List<GameObject> _prefabList = new List<GameObject>();
            GameObject _prefab;
            foreach (var _path in files) {
                _prefab = AssetDatabase.LoadAssetAtPath(_path, typeof(GameObject)) as GameObject;
                _prefabList.Add(_prefab);
            }
            return _prefabList;
        }

    }
}
