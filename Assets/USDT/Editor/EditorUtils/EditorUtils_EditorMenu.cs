using Bujuexiao.Editor;
using MessagePack;
using RobotCat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using USDT.CustomEditor.CompileSound;
using USDT.CustomEditor.Excel;
using USDT.Utils;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
using System.Net;

namespace USDT.CustomEditor {

    // Editor Menu
    public static class EditorUtils_EditorMenu {

        [MenuItem("EditorUtils/测试")]
        private static void Test() {


        }


        [MenuItem("EditorUtils/Path/OpenPersistentDataPath")]
        private static void OpenPersistentDataPath() {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        [MenuItem("EditorUtils/Windows/SearchTexture")]
        private static void OpenSearchTextureWindow() {
            EditorWindow.GetWindowWithRect<SearchImageWindow>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/EditorStyleViewer")]
        private static void OpenEditorStyleViewer() {
            EditorWindow.GetWindowWithRect<EditorStyleViewer>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/BJXEditorWindow")]
        private static void OpenBJXEditorWindow() {
            EditorWindow.GetWindowWithRect<BJXEditorWindow>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/ControlDragWindow")]
        private static void OpenControlDragWindow() {
            EditorWindow.GetWindowWithRect<ControlDragWnd>(GetScreenCenterRect(800, 500)).Show();
        }

        [MenuItem("EditorUtils/Windows/NodeEditorWindow")]
        private static void OpenNodeEditorWindow() {
            EditorWindow.GetWindow<NodeEditorWindow>().Show();
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

        #region 脚本编译

        [MenuItem("EditorUtils/Compilation/强制请求编译脚本")]
        public static void RequestScriptReload() {
            CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active)]
        public static void ToggleWaitMusic() {
            CompileSoundSettings.UseWaitMusic ^= true;
            CompileSoundListener.UpdateToggle();
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Active, true)]
        public static bool CheckToggleWaitMusic() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Active, CompileSoundSettings.UseWaitMusic);
            return true;
        }


        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle)]
        public static void ChooseShuffle() {
            CompileSoundSettings.Shuffle ^= true;
        }
        [MenuItem(EditorMenuConst.CompileSound_MenuItemName_Shuffle, true)]
        public static bool CheckChooseShuffle() {
            Menu.SetChecked(EditorMenuConst.CompileSound_MenuItemName_Shuffle, CompileSoundSettings.Shuffle);
            return true;
        }
        #endregion

        #region 工具

        private static Rect GetScreenCenterRect(float width, float height) {
            int w = Screen.currentResolution.width;
            int h = Screen.currentResolution.height;
            if (h > w) {
                // OnEnd Display roation
                w = Screen.currentResolution.height;
                h = Screen.currentResolution.width;
            }

            return new Rect(w / 2 - width * 0.5f, h / 2 - height * 0.5f, width, height);
        }


        #endregion

        #region 右键
        [MenuItem("Assets/GenerateNameSpace", false)]
        private static void GenerateNameSpace() {
            if(Selection.assetGUIDs == null || Selection.assetGUIDs.Length != 1) {
                lg.e("请选择Asset");
                return;
            }

            var guid = Selection.assetGUIDs[0];
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(assetPath)) {
                lg.e("选择的是非目录");
                return;
            }

            var @namespace = new DirectoryInfo(assetPath).Name;

            var files = Directory.GetFiles(assetPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files) {
                GenerateNamespaceSingleFile(file, @namespace);
            }

            AssetDatabase.Refresh();
        }

		[MenuItem("Assets/ShowAssetText", false)]
		private static void ShowAssetText() {
			if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length != 1) {
				lg.e("请选择Asset");
				return;
			}
			var guid = Selection.assetGUIDs[0];
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var text = File.ReadAllText(assetPath);
			AssetTextViewer.Open(text);
		}

		private static void GenerateNamespaceSingleFile(string filePath, string @namespace) {
            var allText = File.ReadAllText(filePath);
            if (allText.Contains("namespace")) {
				return;
            }
            string[] lines = File.ReadAllLines(filePath, Encoding.GetEncoding("GB2312"));

            // 分离 using 语句和命名空间内容
            var usingStatements = lines.TakeWhile(line => line.TrimStart().StartsWith("using")).ToList();
            var namespaceContent = lines.Skip(usingStatements.Count).ToList();

            // 添加新的命名空间
            var newContent = new StringBuilder();
            usingStatements.ForEach(line => newContent.AppendLine(line));
            newContent.AppendLine("\n");
            newContent.AppendLine($"namespace {@namespace} {{");
            namespaceContent.ForEach(line => {
                if (!string.IsNullOrEmpty(line)) {
                    newContent.AppendLine(line);
                }
            });
            newContent.AppendLine("}");

            // 写回文件
            File.WriteAllText(filePath, newContent.ToString(), Encoding.UTF8);
        }

        #endregion
    }
}