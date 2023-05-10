using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.Utils;
using USDT.Extensions;
using LitJson;
using System.IO;
using System.Text;
using System;
using System.Linq;
using UnityEngine.UI;

namespace USDT.CustomEditor {

    public static class EditorMenuUtils {

        [MenuItem("EditorUtils/测试")]
        private static void Test() {
            var prefabs = EditorUtils.GetAllPrefabByAssetDatabase("Assets");
            foreach (var prefab in prefabs) {
                if (prefab == null) {
                    continue;
                }
                var path = AssetDatabase.GetAssetPath(prefab.GetInstanceID());
                var missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(prefab);
                if (missingScriptCount > 0) {
                    Debug.LogError($"[CheckMaterialsMissing] 预制体:{path},丢失脚本数量:{missingScriptCount}");
                }
                var renders = prefab.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renders) {
                    foreach (var rMat in renderer.sharedMaterials) {
                        if (rMat == null) {
                            Debug.LogError($"[CheckMaterialsMissing] Renderer材质丢失：{path}");
                        }
                    }
                }
            }
        }

        [MenuItem("EditorUtils/Path/OpenPersistentDataPath")]
        private static void OpenPersistentDataPath() {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("EditorUtils/Windows/SearchTexture")]
        private static void OpenSearchTextureWindow() {
            EditorWindow.GetWindow<SearchImageWindow>();
        }

        [MenuItem("EditorUtils/Windows/EditorStyleViewer")]
        private static void OpenEditorStyleViewer() {
            EditorWindow.GetWindow<EditorStyleViewer>().Close();
            EditorWindow.GetWindowWithRect<EditorStyleViewer>(EditorMenuConst.ScreenCenterRect);
        }
    }
}