using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorUtils {

    public static class MenuTools {

        [MenuItem("Tools/测试")]
        public static void Test() {
            //Debug.LogError(GUIUtility.GetControlID(FocusType.Passive));

            EditorWindow.GetWindow<ControlDragWnd>().Show();
        }

    }
}