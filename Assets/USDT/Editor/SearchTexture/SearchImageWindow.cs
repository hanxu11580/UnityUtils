using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using USDT.Utils;
using System;

namespace USDT.CustomEditor {
    /// <summary>
    /// 查找Texuture
    /// </summary>
    public class SearchImageWindow : EditorWindow {

        private enum ESearchMode {
            大小,
            名字含有中文
        }


        // 预设文件名后缀
        private const string ImgtypeStr = "*.BMP|*.JPG|*.GIF|*.PNG|*.PSD";
        private string[] _imageType = { };
        private List<string> _allImagePaths = new List<string>();

        private int _greaterThanSize = int.MaxValue;
        private int _lessThanSize = int.MaxValue;
        private List<string> _matchImagePaths = new List<string>();
        private Vector2 _matchImageScrollViewPos = new Vector2();

        //搜索模式
        private ESearchMode _mode;

        private void OnEnable() {
            _imageType = ImgtypeStr.Split('|');
        }


        private void OnGUI() {
            EditorGUILayout.Space(20);
            EditorGUI.indentLevel++;
            _mode = (ESearchMode)EditorGUILayout.EnumPopup(_mode, GUILayout.Width(200));
            EditorGUILayout.BeginHorizontal(GUIStyleConst.BoxGroup);
            DrawFromMode(_mode);
            GUILayout.FlexibleSpace();
            // 右框 
            if (GUILayout.Button(EditorUtils.TempContent("  搜索  "), GUILayout.Width(100), GUILayout.Height(35))) {
                Clear();
                SearchMatchImage(_mode);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            // 搜索内容
            _matchImageScrollViewPos = EditorGUILayout.BeginScrollView(_matchImageScrollViewPos, GUIStyleConst.BoxGroup);
            DrawMatchImage();
            EditorGUILayout.EndScrollView();
        }

        private void SearchMatchImage(ESearchMode mode) {
            for (int i = 0; i < _imageType.Length; i++) {
                //获取Application.dataPath文件夹下所有的图片路径
                _allImagePaths.AddRange(Directory.GetFiles("Assets/", _imageType[i], SearchOption.AllDirectories));
            }

            for (int i = 0; i < _allImagePaths.Count; i++) {
                var p = _allImagePaths[i];
                if (mode == ESearchMode.大小) {
                    var size = FileUtils.GetFileSize(p) / 1024;
                    if (size >= _greaterThanSize && size <= _lessThanSize) {
                        if (!_matchImagePaths.Contains(p)) {
                            _matchImagePaths.Add(p);
                        }
                    }
                } else if (mode == ESearchMode.名字含有中文) {
                    if (RegexUtils.HasChinese(p)) {
                        _matchImagePaths.Add(p);
                    }
                }
            }
        }

        private void DrawMatchImage() {
            foreach (var p in _matchImagePaths) {
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p);
                var name = Path.GetFileName(p);
                EditorGUILayout.ObjectField(EditorUtils.TempContent(name), obj, typeof(UnityEngine.Object));
            }
        }

        private void DrawFromMode(ESearchMode mode) {
            EditorGUILayout.BeginHorizontal(GUIStyleConst.BoxGroup);
            switch (mode) {
                case ESearchMode.名字含有中文: {
                        if (GUILayout.Button(EditorUtils.TempContent("  删除所有中文图片  "), GUILayout.Width(200), GUILayout.Height(35))) {
                            DeleteMatchImage();
                        }
                        break;
                    }
                case ESearchMode.大小: {
                        // 图片大小范围
                        _greaterThanSize = EditorGUILayout.IntField(_greaterThanSize, GUILayout.Width(100));
                        EditorGUILayout.LabelField(EditorUtils.TempContent("～"), GUILayout.Width(50));
                        _lessThanSize = EditorGUILayout.IntField(_lessThanSize, GUILayout.Width(100));
                        // 如果出现只想知道大于4M的图片
                        if (_lessThanSize <= 0) {
                            _lessThanSize = int.MaxValue;
                        }
                        EditorGUILayout.LabelField(EditorUtils.TempContent("KB"), GUILayout.Width(50));
                        break;
                    }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DeleteMatchImage() {
            if(_matchImagePaths != null) {
                foreach (var p in _matchImagePaths) {
                    FileUtil.DeleteFileOrDirectory(p);
                }
                Clear();
                AssetDatabase.Refresh();
            }
        }

        private void Clear() {
            _allImagePaths.Clear();
            _matchImagePaths.Clear();
        }
    }
}