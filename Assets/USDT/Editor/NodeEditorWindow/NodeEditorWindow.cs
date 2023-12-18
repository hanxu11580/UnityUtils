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

    // 移出线段模式
    private bool _isRemoveConnectionMode;
    // 网格拖拽偏移
    private Vector2 _gridOffset;
    // 右拽拖拽中
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
                        // 直接重画，不然会有延迟
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
            menuItems = new string[] { "删除节点" };
            menuActionItems = new MenuFunction[] { 
                () => RemoveNode(inRangeNode) 
            };
        }
        else {
            menuItems = new string[] { "添加节点" };
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
        // 移出node需要处理线段
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

        // 两种情况要避免
        // 1. 自己连自己
        // 2. 重复连线
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
        //宽度分段
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        //高度分段
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();//在 3D Handle GUI 内开始一个 2D GUI 块。
        {
            //设置颜色：
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            //单格的偏移，算是GridOffset的除余
            Vector3 gridOffset = new Vector3(_gridOffset.x % gridSpacing, _gridOffset.y % gridSpacing, 0);

            //绘制所有的竖线
            for (int i = 0; i < widthDivs; i++) {
                Handles.DrawLine(
                    new Vector3(gridSpacing * i, 0 - gridSpacing, 0) + gridOffset,                  //起点
                    new Vector3(gridSpacing * i, position.height + gridSpacing, 0f) + gridOffset);  //终点
            }
            //绘制所有的横线
            for (int j = 0; j < heightDivs; j++) {
                Handles.DrawLine(
                    new Vector3(0 - gridSpacing, gridSpacing * j, 0) + gridOffset,                  //起点
                    new Vector3(position.width + gridSpacing, gridSpacing * j, 0f) + gridOffset);   //终点
            }

            //重设颜色
            Handles.color = Color.white;
        }
        Handles.EndGUI(); //结束一个 2D GUI 块并返回到 3D Handle GUI。
    }
}
