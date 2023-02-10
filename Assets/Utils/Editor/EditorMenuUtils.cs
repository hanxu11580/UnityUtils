using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using Utility;
using NPOI.SS.UserModel;
using System;

namespace EditorUtils {

    public static class EditorMenuUtils {

        public static Rect ScreenCenterRect
            = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.5f, Screen.height * 0.5f);


        [MenuItem("EditorUtils/测试")]
        private static void Test() {
            //EditorWindow.GetWindow<ControlDragWnd>().Show();

            var dobjd = Utility.UniversalUtils.DeserializeFromPath<Dictionary<string, string>>(@"C:\Users\A\Desktop\temp.data");
            //foreach (var kv in dobjd) {
            //    Debug.LogError($"{kv.Key}-{kv.Value}");
            //}

            var dobj = Utility.UniversalUtils.DeserializeFromPath<List<string>>(@"C:\Users\A\Desktop\use.data");
            foreach (var kv in dobj) {
                var replace = kv.Replace(".bundle", "");
                string[] s2 = replace.Split('_');
                string[] news = new string[s2.Length - 1];
                Array.Copy(s2, 0, news, 0, s2.Length - 1);
                var p = string.Join('_', news);
                p += ".bundle";
                if (dobjd.ContainsKey(p)) {
                    Debug.LogError($"{p}-{dobjd[p]}");
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
            EditorWindow.GetWindowWithRect<EditorStyleViewer>(ScreenCenterRect);
        }

        private static string path = @"‪C:\Users\A\Desktop\2023.02.10 15_40_56 8e0deccf-8f07-4d4c-b3cd-97af66b2a435.csv";

        private static void Process() {
            IWorkbook book = ExcelUtils.LoadBook(path);
            ISheet sheet = book.GetSheetAt(0);


        }
    }
}