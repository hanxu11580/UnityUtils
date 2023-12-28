using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Text;
namespace USDT.Components {
    public class UIRebuildLogger : MonoBehaviour {

        /// <summary>
        /// �Ƿ���֡ģʽ��
        /// ֡ģʽ�¹���ÿһ֡����ЩUIԪ�ش�����Rebuild��
        /// ��֡ģʽ�������UIԪ�ش����˶��ٴ�Rebuild�����鿪��Console��Collapse��
        /// </summary>
        [Tooltip("�Ƿ���֡ģʽ��\n֡ģʽ�¹���ÿһ֡����ЩUIԪ�ش�����Rebuild��\n��֡ģʽ�������UIԪ�ش����˶��ٴ�Rebuild�����鿪��Console��Collapse��")]
        [SerializeField]
        bool m_FrameMode;

        IList<ICanvasElement> m_LayoutRebuildQueue;
        IList<ICanvasElement> m_GraphicRebuildQueue;
        StringBuilder sb = new StringBuilder();
        private void Awake() {
            Type type = typeof(CanvasUpdateRegistry);
            FieldInfo field = type.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            m_LayoutRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
            field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            m_GraphicRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
        }

        private void Update() {


            for (int j = 0; j < m_LayoutRebuildQueue.Count; j++) {
                ICanvasElement element = m_LayoutRebuildQueue[j];
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

                string str = $"<color=#ff0000>{element.transform.name}</color>��LayoutRebuild����<color=#ff0000>{canvas.name}</color>�����ؽ�";

                if (m_FrameMode) {
                    sb.AppendLine(str);
                }
                else {
                    Debug.LogError(str);
                }

            }

            for (int j = 0; j < m_GraphicRebuildQueue.Count; j++) {
                ICanvasElement element = m_GraphicRebuildQueue[j];

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

                string str = $"<color=#ff0000>{element.transform.name}</color>��LayoutRebuild����<color=#ff0000>{canvas.name}</color>�����ؽ�";

                if (m_FrameMode) {
                    sb.AppendLine(str);
                }
                else {
                    Debug.LogError(str);
                }
            }

            if (sb.Length != 0) {
                Debug.LogError($"��ǰ֡<color=#66ccff>{Time.frameCount}</color>:\n{sb.ToString()}");
                sb.Clear();
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
