using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using USDT.CustomEditor;
using USDT.Utils;

namespace Bujuexiao.Editor {
    public class BJXEditorWindow : EditorWindow {

        private const string _DragRegionText = "���ļ����������";
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

                    // �༭���ڲ��ļ������ҿ�ʶ���ļ�
                    foreach (var draggedObject in DragAndDrop.objectReferences) {
                        TextAsset textFile = draggedObject as TextAsset;
                        if (textFile != null) {
                            LogUtils.Log($"����:{textFile.name}");
                        }
                    }

                    // �༭���ⲿ�ļ�����༭���޷�ʶ����ļ�
                    foreach (var path in DragAndDrop.paths) {
                        LogUtils.Log($"����:{path}");
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