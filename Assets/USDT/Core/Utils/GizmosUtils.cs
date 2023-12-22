using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Utils {
    public static class GizmosUtils {
        public static void DrawRect(Rect rect) {
            // 计算矩形的四个角
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin, 0);
            // 画出矩形的四条边

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}