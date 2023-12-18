using UnityEditor;
using UnityEngine;
public class Node {
    public Rect rect;
    public ConnectionPoint inConnectionPoint;
    public ConnectionPoint outConnectionPoint;

    private bool _isDragged;
    public Node(Vector2 position) {
        rect = new Rect(position.x, position.y, NodeEditorWindow.DefaultNodeWidth, NodeEditorWindow.DefaultNodeHeight);
        inConnectionPoint = new ConnectionPoint(this, ConnectionPointType.In);
        outConnectionPoint = new ConnectionPoint(this, ConnectionPointType.Out);
    }

    public void Drag(Vector2 delta) {
        rect.position += delta;
    }

    public void Draw() {
        GUI.Box(rect, "Node", NodeEditorWindow.NodeStyle);
        inConnectionPoint.Draw();
        outConnectionPoint.Draw();
    }

    public void ProcessEvents(Event e, NodeEditorWindow nodeEditorWindow) {
        switch (e.type) {
            case EventType.MouseDown: {
                    // 左键
                    if (e.button == 0) {
                        if (rect.Contains(e.mousePosition)) {
                            _isDragged = true;
                        }
                        else if (inConnectionPoint.rect.Contains(e.mousePosition)) {
                            nodeEditorWindow.MouseDownSelectingPoint = inConnectionPoint;
                            // 上面无法使用e.Use()否则无法拖拽Node
                            e.Use();
                        }
                        else if (outConnectionPoint.rect.Contains(e.mousePosition)) {
                            nodeEditorWindow.MouseDownSelectingPoint = outConnectionPoint;
                            e.Use();
                        }
                        nodeEditorWindow.Repaint();
                    }
                    break;
                }
            case EventType.MouseUp: {
                    _isDragged = false;
                    if (inConnectionPoint.rect.Contains(e.mousePosition)) {
                        nodeEditorWindow.MouseUpSelectingPoint = inConnectionPoint;
                    }
                    else if (outConnectionPoint.rect.Contains(e.mousePosition)) {
                        nodeEditorWindow.MouseUpSelectingPoint = outConnectionPoint;
                    }
                    break;
                }
            case EventType.MouseDrag: {
                    // 左键拖拽
                    if (e.button == 0 && _isDragged) {
                        Drag(e.delta);
                        nodeEditorWindow.Repaint();
                        // 其他GUI元素之后将它忽略
                        e.Use();
                    }
                    break;
                }
        }
    }
}


public enum ConnectionPointType {
    In,
    Out
}

public class ConnectionPoint {
    public Rect rect;
    public ConnectionPointType type;
    public Node rootNode;
    public bool isSelecting;

    public ConnectionPoint(Node node, ConnectionPointType connectionPointType) {
        this.rootNode = node;
        this.type = connectionPointType;

        rect = new Rect (0, 0, 20f, 20f);
    }

    public void Draw() {
        // Node中间位置，然后向上移半个进出点（也就是自己）的高度
        rect.y = rootNode.rect.y + (rootNode.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type) {
            case ConnectionPointType.In: {
                    // Node位置往左移动半个进出点宽度
                    rect.x = rootNode.rect.x - rect.width + 3;
                    break;
                }
            case ConnectionPointType.Out: {
                    rect.x = rootNode.rect.x + rootNode.rect.width - 3;
                    break;
                }
        }

        GUI.Box(rect, "", NodeEditorWindow.PointStyle);
    }
}


public class ConnectionLine {
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public ConnectionLine(ConnectionPoint inPoint, ConnectionPoint outPoint) {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
    }

    public void Draw(bool isRemoveConnectionMode, NodeEditorWindow nodeEditorWindow) {
        Handles.DrawBezier(
            inPoint.rect.center, // 起始
            outPoint.rect.center, // 终点
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null, // 纹理
            5f // 线宽
            );

        if (isRemoveConnectionMode) {
            Vector2 buttonSize = new Vector2(20, 20);
            Vector2 lineCenter = (inPoint.rect.center + outPoint.rect.center) / 2;
            if (GUI.Button(new Rect(lineCenter - buttonSize / 2, buttonSize), "X")) {
                nodeEditorWindow.RemoveLine(this);
            }
        }
    }
}
