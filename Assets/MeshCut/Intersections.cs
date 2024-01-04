using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace MeshCut {
    public class Intersections {
        public static bool BoundPlaneIntersect(Mesh mesh, ref Plane plane) {
            return MathUtils.BoundPlaneIntersect(mesh.bounds, ref plane);
        }

        private readonly Vector3[] v;
        private readonly Vector2[] u;
        private readonly int[] t;
        private readonly bool[] positive;
        private Ray edgeRay;

        public Intersections() {
            v = new Vector3[3];
            u = new Vector2[3];
            t = new int[3];
            positive = new bool[3];
        }

        /// <summary>
        /// 线段相交，返回交点和uv
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="uv1"></param>
        /// <param name="uv2"></param>
        /// <returns></returns>
        public ValueTuple<Vector3, Vector2> Intersect(Plane plane, Vector3 first, Vector3 second, Vector2 uv1, Vector2 uv2) {
            // 用下面图来看 例如
            // P0->P1射线
            edgeRay.origin = first;
            edgeRay.direction = (second - first).normalized;
            float dist;
            // P0->P1距离
            float maxDist = Vector3.Distance(first, second);

            // dist应该是P0-I1的距离
            if (!plane.Raycast(edgeRay, out dist))
                // Intersect in wrong direction...
                throw new UnityException("Line-Plane intersect in wrong direction");
            else if (dist > maxDist)
                // Intersect outside of line segment
                throw new UnityException("Intersect outside of line");

            // I1的坐标
            var returnVal = new ValueTuple<Vector3, Vector2> {
                Item1 = edgeRay.GetPoint(dist)
            };

            var relativeDist = dist / maxDist;
            // I1的uv坐标
            returnVal.Item2.x = Mathf.Lerp(uv1.x, uv2.x, relativeDist);
            returnVal.Item2.y = Mathf.Lerp(uv1.y, uv2.y, relativeDist);
            return returnVal;
        }

        /*
         * 这个类都是基于这个图
         * Small diagram for reference :)
         *       |      |  /|
         *       |      | / |P1       
         *       |      |/  |         
         *       |    I1|   |
         *       |     /|   |
         *      y|    / |   |
         *       | P0/__|___|P2
         *       |      |I2
         *       |      |
         *       |___________________
         */
        /// <summary>
        /// 三角和平面相交
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="uvs"></param>
        /// <param name="triangles"></param>
        /// <param name="startIdx"></param>
        /// <param name="plane"></param>
        /// <param name="posMesh">被平面分割的mesh</param>
        /// <param name="negMesh">被平面分割的mesh</param>
        /// <param name="intersectVectors"></param>
        /// <returns></returns>
        public bool TrianglePlaneIntersect(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, int startIdx, ref Plane plane, TempMesh posMesh, TempMesh negMesh, Vector3[] intersectVectors) {
            int i;

            // Store triangle, vertex and uv from indices
            for (i = 0; i < 3; ++i) {
                t[i] = triangles[startIdx + i];
                v[i] = vertices[t[i]];
                u[i] = uvs[t[i]];
            }

            // 判断三角形三个顶点是否在posMesh里
            posMesh.ContainsKeys(triangles, startIdx, positive);

            // 如果3个顶点都在一边的mesh里
            if (positive[0] == positive[1] && positive[1] == positive[2]) {
                // 都在posMesh里 将三角形加入posMesh中
                // 都不在则加入negMesh中
                (positive[0] ? posMesh : negMesh).AddOgTriangle(t);
                return false;
            }

            // 三个点，寻找孤点（比如posMesh上有2个点，negMesh上有1个点，negMesh的这一个点就是孤点）
            int lonelyPoint = 0;
            if (positive[0] != positive[1])
                lonelyPoint = positive[0] != positive[2] ? 0 : 1;
            else
                lonelyPoint = 2;



            // 孤点的前一个顶点索引
            int prevPoint = lonelyPoint - 1;
            // 如果是0，前一个顶点就是2
            if (prevPoint == -1) prevPoint = 2;
            // 孤点的下一个顶点索引
            int nextPoint = lonelyPoint + 1;
            if (nextPoint == 3) nextPoint = 0;

            // 最上面那个图
            // 假设P0是孤点
            // P0_P2线段和屏幕相交交点 I2
            ValueTuple<Vector3, Vector2> newPointPrev = Intersect(plane, v[lonelyPoint], v[prevPoint], u[lonelyPoint], u[prevPoint]);
            // P0_P1线段和屏幕相交交点 I1
            ValueTuple<Vector3, Vector2> newPointNext = Intersect(plane, v[lonelyPoint], v[nextPoint], u[lonelyPoint], u[nextPoint]);

            // 设置新的三角形，并放到对应的TempMesh里

            // 3个都是顺时针
            // 三角形 P0-I1-I2
            (positive[lonelyPoint] ? posMesh : negMesh).AddSlicedTriangle(t[lonelyPoint], newPointNext.Item1, newPointPrev.Item1, newPointNext.Item2, newPointPrev.Item2);
            // 三角形 P2-I2-P1
            (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[prevPoint], newPointPrev.Item1, newPointPrev.Item2, t[nextPoint]);
            // 三角形 P1-I2-I1
            (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[nextPoint], newPointPrev.Item1, newPointNext.Item1, newPointPrev.Item2, newPointNext.Item2);

            if (positive[lonelyPoint]) {
                // 如果孤点在posMesh I2、I1
                intersectVectors[0] = newPointPrev.Item1;
                intersectVectors[1] = newPointNext.Item1;
            }
            else {
                // 孤点在negMesh I1、I2
                intersectVectors[0] = newPointNext.Item1;
                intersectVectors[1] = newPointPrev.Item1;
            }
            return true;
        }
    }
}