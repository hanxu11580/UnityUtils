using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace USDT.CustomEditor {
    public class AssetTextViewer : EditorWindow{

        private static string _text;
        private Vector2 _scrollPosition;


        public static void Open(string text) {
            _text = text;
            GetWindow<AssetTextViewer>().Show();
        }

        private void OnGUI() {
            GUILayout.Space(20);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.TextField(_text);
            GUILayout.EndScrollView();
        }
    }
}