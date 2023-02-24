using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.Utils;
using USDT.Extensions;
namespace USDT.CustomEditor {

    public static class EditorMenuUtils {

        [MenuItem("EditorUtils/测试")]
        private static void Test() {

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