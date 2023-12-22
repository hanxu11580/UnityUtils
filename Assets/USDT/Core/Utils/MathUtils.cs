using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Utils {
    public static class MathUtils
    {
        /// <summary> 是否为偶数 </summary>
        public static bool IsEvennumber(int val)
        {
            return (val & 1) == 0; //与1进行二进制 二进制偶数最后一位为 0
        }

        /// <summary>
        /// 线段是否和某Rect相交
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool LineIntersectsRect(Vector2 a, Vector2 b, Rect r) {
            var minX = Math.Min(a.x, b.x);
            var maxX = Math.Max(a.x, b.x);
            var minY = Math.Min(a.y, b.y);
            var maxY = Math.Max(a.y, b.y);

            if (r.xMin > maxX || r.xMax < minX) {
                return false;
            }

            if (r.yMin > maxY || r.yMax < minY) {
                return false;
            }

            if (r.xMin < minX && maxX < r.xMax) {
                return true;
            }

            if (r.yMin < minY && maxY < r.yMax) {
                return true;
            }

            Func<float, float> yForX = x => a.y - (x - a.x) * ((a.y - b.y) / (b.x - a.x));

            var yAtRectLeft = yForX(r.xMin);
            var yAtRectRight = yForX(r.xMax);

            if (r.yMax < yAtRectLeft && r.yMax < yAtRectRight) {
                return false;
            }

            if (r.yMin > yAtRectLeft && r.yMin > yAtRectRight) {
                return false;
            }

            return true;
        }

        // 检查两条线段是否相交
        public static bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
            // 计算交点
            float d = (a1.x - a2.x) * (b2.y - b1.y) - (a1.y - a2.y) * (b2.x - b1.x);
            if (d == 0) return false; // 平行线

            float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
            float v = ((b1.x - a1.x) * (a1.y - a2.y) - (b1.y - a1.y) * (a1.x - a2.x)) / d;

            // 如果交点在两条线段上，则线段相交
            return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
        }

        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static float PointToLineDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd) {
            Vector2 lineVec = lineEnd - lineStart;
            Vector2 pointVec = point - lineStart;

            float areaTimesTwo = Mathf.Abs(lineVec.x * pointVec.y - lineVec.y * pointVec.x);
            float lineLength = lineVec.magnitude;

            float distance = areaTimesTwo / lineLength;
            return distance;
        }
    }

}
