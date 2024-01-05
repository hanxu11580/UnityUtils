using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Text;
using System.Linq;

namespace USDT.Components {
    public class UIRebuildLogger : MonoBehaviour {

        IList<ICanvasElement> _layoutRebuildQueue;
        IList<ICanvasElement> _graphicRebuildQueue;
        Dictionary<string, int> _map = new Dictionary<string, int>();
        StringBuilder _sb = new StringBuilder();
        private void Awake() {
            Type type = typeof(CanvasUpdateRegistry);
            FieldInfo field = type.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            _layoutRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
            field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            _graphicRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
        }

        private void Update() {

            try {
                for (int j = 0; j < _layoutRebuildQueue.Count; j++) {
                    ICanvasElement element = _layoutRebuildQueue[j];
                    if (!ObjectValidForUpdata(element)) {
                        continue;
                    }

                    if (element.transform == null) {
                        continue;
                    }
                    Graphic graphic = element.transform.GetComponent<Graphic>();
                    if (graphic == null) {
                        continue;
                    }

                    Canvas canvas = graphic.canvas;
                    if (canvas == null) {
                        continue;
                    }

                    string str = $"<color=#ff0000>{element.transform.name}</color>的LayoutRebuild引起<color=#ff0000>{canvas.name}</color>网格重建";

                    if (_map.ContainsKey(str)) {
                        _map[str] += 1;
                    }
                    else {
                        _map.Add(str, 1);
                    }
                }

                for (int j = 0; j < _graphicRebuildQueue.Count; j++) {
                    ICanvasElement element = _graphicRebuildQueue[j];

                    if (!ObjectValidForUpdata(element)) {
                        continue;
                    }

                    Graphic graphic = element.transform.GetComponent<Graphic>();
                    if (graphic == null) {
                        continue;
                    }

                    Canvas canvas = graphic.canvas;
                    if (canvas == null) {
                        continue;
                    }


                    string canvansName = canvas.name;
                    if (canvansName == "DebugFps") {
                        continue;
                    }

                    string str = $"<color=#ff0000>{element.transform.name}</color>的LayoutRebuild引起<color=#ff0000>{canvas.name}</color>网格重建";

                    if (_map.ContainsKey(str)) {
                        _map[str] += 1;
                    }
                    else {
                        _map.Add(str, 1);
                    }
                }

                if (_map.Count > 0) {
                    _sb.Append($"当前帧<color=#66ccff>{Time.frameCount}</color> 重建总次数:{_map.Values.Sum()}\n");
                    foreach (var kv in _map) {
                        _sb.AppendLine($"{kv.Key} 重建次数:{kv.Value}");
                    }
                    Debug.LogError(_sb.ToString());
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
            finally {
                _map.Clear();
                _sb.Clear();
            }
        }

        private bool ObjectValidForUpdata(ICanvasElement element) {
            bool valid = element != null;
            bool isUnityObject = element is UnityEngine.Object;

            if (isUnityObject) {
                valid = (element as UnityEngine.Object) != null;
            }

            return valid;
        }
    }
}
