using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace USDT.CustomEditor {

    public static class EditorWindowExtensions {

        /// <summary>
        /// ´°¿Ú»­¼ÓÔØ»­Ãæ
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="iconSize"></param>
        public static void DrawLoadingTex(this EditorWindow wnd, int iconSize) {
            var loadingImageBig = EditorUtils.FindBuiltinTexture("EditorSettings Icon");
            var oldColor = GUI.color;
            var oldMatrix = GUI.matrix;

            var pivotPoint = new Vector2(wnd.position.width / 2, wnd.position.height / 2);
            GUIUtility.RotateAroundPivot(Time.realtimeSinceStartup * 45, pivotPoint);

            var r = new Rect(pivotPoint - new Vector2(0.5f, 0.5f) * iconSize, Vector2.one * iconSize);
            GUI.color = new Color(1, 1, 1, 1.0f);
            GUI.DrawTexture(r, loadingImageBig);

            GUI.matrix = oldMatrix;
            GUI.color = oldColor;
        }
    }
}