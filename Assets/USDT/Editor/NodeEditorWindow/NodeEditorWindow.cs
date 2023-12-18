using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.CustomEditor;
using USDT.Expand;
using static UnityEditor.GenericMenu;

public class NodeEditorWindow : EditorWindow {
    public const int DefaultNodeWidth = 160;
    public const int DefaultNodeHeight = 100;

    public static GUIStyle PointStyle;
    public static GUIStyle NodeStyle;

    public ConnectionPoint MouseDownSelectingPoint { get; set; }
    public ConnectionPoint MouseUpSelectingPoint { get; set; }

    private List<Node> _nodes;
    private List<ConnectionLine> _connectionLines;

    // �Ƴ��߶�ģʽ
    private bool _isRemoveConnectionMode;
    // ������קƫ��
    private Vector2 _gridOffset;
    // ��ק��ק��
    private bool _isRightMouseDragging;

    private void OnEnable() {
        _nodes = new List<Node>();
        _connectionLines = new List<ConnectionLine>();

        PointStyle = new GUIStyle();
        PointStyle.normal.background = EditorGUIUtility.Load("Assets/USDT/Editor/NodeEditorWindow/Arts/Circle.png") as Texture2D;

        NodeStyle = new GUIStyle();
        NodeStyle.alignment = TextAnchor.MiddleCenter;
        NodeStyle.normal.textColor = Color.white;
        NodeStyle.fontStyle = FontStyle.BoldAndItalic;
        NodeStyle.normal.background = EditorGUIUtility.Load("Assets/USDT/Editor/NodeEditorWindow/Arts/Box.png") as Texture2D;
    }

    private void OnGUI() {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawNodes();
        DrawLines();
        DrawPendingConnection(Event.current);
        ProcessEvents(Event.current);
    }

    private void ProcessEvents(Event e) {
        switch (e.type) {
            case EventType.MouseDrag: {
                    if (e.button == 1) {
                        DragNodes(e.delta);
                        _gridOffset += e.delta;
                        _isRightMouseDragging = true;
                        Repaint();
                    }
                    break;
                }
            case EventType.MouseUp: {
                    if(e.button == 0) {
                        MouseDownSelectingPoint = null;
                        MouseUpSelectingPoint = null;
                        Repaint();
                    }
                    else if(e.button == 1) {
                        if (!_isRightMouseDragging) {
                            ProcessRightMouseDown(e.mousePosition);
                        }
                        _isRightMouseDragging = false;
                    }
                    break;
                }
            case EventType.KeyDown: {
                    if (e.keyCode == KeyCode.R) {
                        _isRemoveConnectionMode = true;
                        // ֱ���ػ�����Ȼ�����ӳ�
                        Repaint();
                    }
                    break;
                }
            case EventType.KeyUp: {
                    if (e.keyCode == KeyCode.R) {
                        _isRemoveConnectionMode = false;
                        Repaint();
                    }
                    break;
                }
        }
    }

    private void ProcessRightMouseDown(Vector2 mousePosition) {
        Node inRangeNode = null;
        foreach (var node in _nodes) {
            if (node.rect.Contains(mousePosition)) {
                inRangeNode = node;
                break;
            }
        }

        string[] menuItems;
        MenuFunction[] menuActionItems;
        if (inRangeNode != null) {
            menuItems = new string[] { "ɾ���ڵ�" };
            menuActionItems = new MenuFunction[] { 
                () => RemoveNode(inRangeNode) 
            };
        }
        else {
            menuItems = new string[] { "��ӽڵ�" };
            menuActionItems = new MenuFunction[] { 
                () => AddNode(mousePosition)
            };
        }

        EditorUtils.DrawMenu(menuItems, menuActionItems);
    }

    private void AddNode(Vector2 position) {
        _nodes.Add(new Node(position));
    }

    private void RemoveNode(Node node) {
        // �Ƴ�node��Ҫ�����߶�
        for (int i = _connectionLines.Count - 1; i >= 0 ; i--) {
            var line = _connectionLines[i];
            if(line.inPoint.rootNode == node || line.outPoint.rootNode == node) {
                _connectionLines.ExRemove(line);
            }
        }
        _nodes.ExRemove(node);
    }

    public void RemoveLine(ConnectionLine line) {
        for (int i = _connectionLines.Count - 1; i >= 0; i--) {
            if (_connectionLines[i] == line) {
                _connectionLines.ExRemove(line);
                return;
            }
        }
    }

    private void DrawNodes() {
        for (int i = 0; i < _nodes.Count; i++) {
            _nodes[i].Draw();
            _nodes[i].ProcessEvents(Event.current, this);
        }

        // �������Ҫ����
        // 1. �Լ����Լ�
        // 2. �ظ�����
        if(MouseDownSelectingPoint != null && MouseUpSelectingPoint != null) {
            if(MouseDownSelectingPoint.type != MouseUpSelectingPoint.type
                && MouseDownSelectingPoint.rootNode != MouseUpSelectingPoint.rootNode) {
                ConnectionLine newLine;
                if(MouseDownSelectingPoint.type == ConnectionPointType.In) {
                    newLine = new ConnectionLine(MouseDownSelectingPoint, MouseUpSelectingPoint);
                }
                else {
                    newLine = new ConnectionLine(MouseUpSelectingPoint, MouseDownSelectingPoint);
                }

                bool isAdd = true;
                foreach (var line in _connectionLines) {
                    if(newLine.inPoint == line.inPoint && newLine.outPoint == line.outPoint) {
                        isAdd = false;
                        break;
                    }
                }

                if (isAdd) {
                    _connectionLines.Add(newLine);
                }
            }

            MouseDownSelectingPoint = null;
            MouseUpSelectingPoint = null;
        }
    }

    private void DragNodes(Vector2 delta) {
        for (int i = 0; i < _nodes.Count; i++) {
            _nodes[i].Drag(delta);
        }
    }

    private void DrawLines() {
        for (int i = _connectionLines.Count - 1; i >= 0; i--) {
            _connectionLines[i].Draw(_isRemoveConnectionMode, this);
        }
    }

    private void DrawPendingConnection(Event e) {
        if(MouseDownSelectingPoint != null) {
            Vector3 startPosition = (MouseDownSelectingPoint.type == ConnectionPointType.In) ? MouseDownSelectingPoint.rect.center : e.mousePosition;
            Vector3 endPosition = (MouseDownSelectingPoint.type == ConnectionPointType.In) ? e.mousePosition : MouseDownSelectingPoint.rect.center;

            Handles.DrawBezier(
            startPosition,
            endPosition,
            startPosition + Vector3.left * 50f,
            endPosition - Vector3.left * 50f,
            Color.white,
            null,
            5f
            );
            Repaint();
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
        //��ȷֶ�
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        //�߶ȷֶ�
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();//�� 3D Handle GUI �ڿ�ʼһ�� 2D GUI �顣
        {
            //������ɫ��
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            //�����ƫ�ƣ�����GridOffset�ĳ���
            Vector3 gridOffset = new Vector3(_gridOffset.x % gridSpacing, _gridOffset.y % gridSpacing, 0);

            //�������е�����
            for (int i = 0; i < widthDivs; i++) {
                Handles.DrawLine(
                    new Vector3(gridSpacing * i, 0 - gridSpacing, 0) + gridOffset,                  //���
                    new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + gridOffset);  //�յ�
            }
            //�������еĺ���
            for (int j = 0; j < heightDivs; j++) {
                Handles.DrawLine(
                    new Vector3(0 - gridSpacing, gridSpacing * j, 0) + gridOffset,                  //���
                    new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + gridOffset);   //�յ�
            }

            //������ɫ
            Handles.color = Color.white;
        }
        Handles.EndGUI(); //����һ�� 2D GUI �鲢���ص� 3D Handle GUI��
    }
}
