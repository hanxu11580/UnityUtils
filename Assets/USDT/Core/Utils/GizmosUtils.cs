using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Utils {
    public static class GizmosUtils {
        public static void DrawRect(Rect rect) {
            // ������ε��ĸ���
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin, 0);
            // �������ε�������

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}