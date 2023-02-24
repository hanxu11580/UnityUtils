using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace USDT.CustomEditor {

    public class ControlDragWnd : EditorWindow {
        Rect _startRect = new Rect(100, 150, 100, 40);

        List<string> _names = new List<string>() {
        "1","2","3","4"
    };

        bool _dragging;

        Vector2 _dragOffset;

        private void OnGUI() {
            for (int i = 0; i < _names.Count; i++) {
                var drawRect = new Rect(_startRect);
                drawRect.y += 100 * i;
                GUI.Box(drawRect, _names[i]);

                var cid = GUIUtility.GetControlID(FocusType.Passive);

                Event e = Event.current;
                switch (Event.current.GetTypeForControl(cid)) {
                    case EventType.Repaint: {

                            if (_dragging && GUIUtility.hotControl == cid) {
                                var dragRect = new Rect();
                                dragRect.position = drawRect.position + _dragOffset - new Vector2(50, 20);
                                dragRect.width = 100;
                                dragRect.height = 40;
                                GUI.Box(dragRect, _names[i]);
                            }

                            break;
                        }

                    case EventType.MouseDown: {
                            if (drawRect.Contains(e.mousePosition)) {
                                _dragging = false;
                                GUIUtility.hotControl = cid;
                                //Debug.Log($"MouseDown::{i}");
                                e.Use();

                            }
                            break;
                        }
                    case EventType.MouseDrag: {
                            if (GUIUtility.hotControl == cid) {
                                //Debug.Log($"MouseDrag::{i}");
                                _dragOffset = e.mousePosition - drawRect.position;
                                _dragging = true;
                                e.Use();
                            }
                            break;
                        }
                    case EventType.MouseUp: {
                            if (GUIUtility.hotControl == cid) {
                                GUIUtility.hotControl = 0;
                                _dragOffset = Vector2.zero;
                                _dragging = false;

                                for (int endIndex = 0; endIndex < _names.Count; endIndex++) {
                                    var endRect = new Rect(_startRect);
                                    endRect.y += 100 * endIndex;

                                    if (endIndex != i
                                        && endRect.Contains(e.mousePosition)) {
                                        var temp = _names[endIndex];
                                        _names[endIndex] = _names[i];
                                        _names[i] = temp;
                                        break;
                                    }
                                }


                                e.Use();
                            }
                            break;
                        }
                }
            }
        }
    }
}
