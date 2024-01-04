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
        /// �߶��ཻ�����ؽ����uv
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="uv1"></param>
        /// <param name="uv2"></param>
        /// <returns></returns>
        public ValueTuple<Vector3, Vector2> Intersect(Plane plane, Vector3 first, Vector3 second, Vector2 uv1, Vector2 uv2) {
            // ������ͼ���� ����
            // P0->P1����
            edgeRay.origin = first;
            edgeRay.direction = (second - first).normalized;
            float dist;
            // P0->P1����
            float maxDist = Vector3.Distance(first, second);

            // distӦ����P0-I1�ľ���
            if (!plane.Raycast(edgeRay, out dist))
                // Intersect in wrong direction...
                throw new UnityException("Line-Plane intersect in wrong direction");
            else if (dist > maxDist)
                // Intersect outside of line segment
                throw new UnityException("Intersect outside of line");

            // I1������
            var returnVal = new ValueTuple<Vector3, Vector2> {
                Item1 = edgeRay.GetPoint(dist)
            };

            var relativeDist = dist / maxDist;
            // I1��uv����
            returnVal.Item2.x = Mathf.Lerp(uv1.x, uv2.x, relativeDist);
            returnVal.Item2.y = Mathf.Lerp(uv1.y, uv2.y, relativeDist);
            return returnVal;
        }

        /*
         * ����඼�ǻ������ͼ
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
        /// ���Ǻ�ƽ���ཻ
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="uvs"></param>
        /// <param name="triangles"></param>
        /// <param name="startIdx"></param>
        /// <param name="plane"></param>
        /// <param name="posMesh">��ƽ��ָ��mesh</param>
        /// <param name="negMesh">��ƽ��ָ��mesh</param>
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

            // �ж����������������Ƿ���posMesh��
            posMesh.ContainsKeys(triangles, startIdx, positive);

            // ���3�����㶼��һ�ߵ�mesh��
            if (positive[0] == positive[1] && positive[1] == positive[2]) {
                // ����posMesh�� �������μ���posMesh��
                // �����������negMesh��
                (positive[0] ? posMesh : negMesh).AddOgTriangle(t);
                return false;
            }

            // �����㣬Ѱ�ҹµ㣨����posMesh����2���㣬negMesh����1���㣬negMesh����һ������ǹµ㣩
            int lonelyPoint = 0;
            if (positive[0] != positive[1])
                lonelyPoint = positive[0] != positive[2] ? 0 : 1;
            else
                lonelyPoint = 2;



            // �µ��ǰһ����������
            int prevPoint = lonelyPoint - 1;
            // �����0��ǰһ���������2
            if (prevPoint == -1) prevPoint = 2;
            // �µ����һ����������
            int nextPoint = lonelyPoint + 1;
            if (nextPoint == 3) nextPoint = 0;

            // �������Ǹ�ͼ
            // ����P0�ǹµ�
            // P0_P2�߶κ���Ļ�ཻ���� I2
            ValueTuple<Vector3, Vector2> newPointPrev = Intersect(plane, v[lonelyPoint], v[prevPoint], u[lonelyPoint], u[prevPoint]);
            // P0_P1�߶κ���Ļ�ཻ���� I1
            ValueTuple<Vector3, Vector2> newPointNext = Intersect(plane, v[lonelyPoint], v[nextPoint], u[lonelyPoint], u[nextPoint]);

            // �����µ������Σ����ŵ���Ӧ��TempMesh��

            // 3������˳ʱ��
            // ������ P0-I1-I2
            (positive[lonelyPoint] ? posMesh : negMesh).AddSlicedTriangle(t[lonelyPoint], newPointNext.Item1, newPointPrev.Item1, newPointNext.Item2, newPointPrev.Item2);
            // ������ P2-I2-P1
            (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[prevPoint], newPointPrev.Item1, newPointPrev.Item2, t[nextPoint]);
            // ������ P1-I2-I1
            (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[nextPoint], newPointPrev.Item1, newPointNext.Item1, newPointPrev.Item2, newPointNext.Item2);

            if (positive[lonelyPoint]) {
                // ����µ���posMesh I2��I1
                intersectVectors[0] = newPointPrev.Item1;
                intersectVectors[1] = newPointNext.Item1;
            }
            else {
                // �µ���negMesh I1��I2
                intersectVectors[0] = newPointNext.Item1;
                intersectVectors[1] = newPointPrev.Item1;
            }
            return true;
        }
    }
}