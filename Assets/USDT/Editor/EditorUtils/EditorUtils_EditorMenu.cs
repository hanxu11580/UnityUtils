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
    // Editor Menu
    public static partial class EditorUtils {

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

        #region Excel

        [MenuItem("EditorUtils/Excels/导出表类CS、数据Json")]
        private static void ExportALL() {
            ExcelExportUtil.ExportScriptALL();
            ExcelExportUtil.ExportDataALL();
        }
        [MenuItem("EditorUtils/Excels/导出表类CS")]
        private static void ExportScript() {
            ExcelExportUtil.ExportScriptALL();
        }
        [MenuItem("EditorUtils/Excels/导出表数据")]
        private static void ExportData() {
            ExcelExportUtil.ExportDataALL();
        }
        #endregion
    }
}