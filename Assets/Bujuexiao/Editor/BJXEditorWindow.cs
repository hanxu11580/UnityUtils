using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using USDT.CustomEditor;
using USDT.Utils;

namespace Bujuexiao.Editor {
    public class BJXEditorWindow : EditorWindow {

        private const string _DragRegionText = "将文件拖入此区域";
        private Texture2D _dragRegionStyleBgTex2D;
        private GUIStyle _dragRegionStyle;
        private BJXSaleExcelProcessing _saleExcelProcessing;

        [MenuItem("EditorUtils/Windows/BJXEditorWindow")]
        private static void OpenEditorStyleViewer() {
            GetWindow<BJXEditorWindow>().Close();
            GetWindowWithRect<BJXEditorWindow>(EditorMenuConst.ScreenCenterRect);
        }

        private void OnEnable() {
            _dragRegionStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            _dragRegionStyleBgTex2D = EditorUtils.MakeTex(20, 100, Color.gray);
            _dragRegionStyle.normal.background = _dragRegionStyleBgTex2D;
            _dragRegionStyle.normal.textColor = Color.cyan;
        }

        private void OnDestroy() {
            DestroyImmediate(_dragRegionStyleBgTex2D);
        }

        private void OnGUI() {
            var content = EditorUtils.TempContent(_DragRegionText);
            Rect dropArea = GUILayoutUtility.GetRect(20f, 100f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, content, _dragRegionStyle);

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform) {
                if (!dropArea.Contains(currentEvent.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();

                    // 编辑器内部文件，并且可识别文件
                    foreach (var draggedObject in DragAndDrop.objectReferences) {
                        TextAsset textFile = draggedObject as TextAsset;
                        if (textFile != null) {
                            LogUtils.Log($"拖入:{textFile.name}");
                        }
                    }

                    // 编辑器外部文件，或编辑器无法识别的文件
                    foreach (var path in DragAndDrop.paths) {
                        LogUtils.Log($"拖入:{path}");
                        _saleExcelProcessing = new BJXSaleExcelProcessing(path);
                        _saleExcelProcessing.Processing();
                        _saleExcelProcessing.CalcTotalIncome();
                        _saleExcelProcessing.CalcCommissions();
                    }
                }

                Event.current.Use();
            }
        }
    }
}