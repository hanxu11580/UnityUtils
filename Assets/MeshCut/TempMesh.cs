using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCut {
    public class TempMesh {
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector2> uvs;
        public List<int> triangles;
        /// <summary>
        /// 因为vertices只有原来mesh顶点的一部分
        /// 所以传入原来mesh顶点索引，直接去vertices找不到的，需要又映射表
        /// key: 原mesh顶点索引
        /// value: TempMesh顶点的索引 vertices
        /// </summary>
        private Dictionary<int, int> _vMapping;
        public float surfacearea;

        public TempMesh(int vertexCapacity) {
            vertices = new List<Vector3>(vertexCapacity);
            normals = new List<Vector3>(vertexCapacity);
            uvs = new List<Vector2>(vertexCapacity);
            triangles = new List<int>(vertexCapacity * 3);
            _vMapping = new Dictionary<int, int>(vertexCapacity);
            surfacearea = 0;
        }

        public void Clear() {
            vertices.Clear();
            normals.Clear();
            uvs.Clear();
            triangles.Clear();
            _vMapping.Clear();
            surfacearea = 0;
        }

        private void AddPoint(Vector3 point, Vector3 normal, Vector2 uv) {
            triangles.Add(vertices.Count);
            vertices.Add(point);
            normals.Add(normal);
            uvs.Add(uv);
        }


        public void AddOgTriangle(int[] indices) {
            for (int i = 0; i < 3; ++i)
                triangles.Add(_vMapping[indices[i]]);
            // 计算新加的三角形面积
            surfacearea += GetTriangleArea(triangles.Count - 3);
        }

        public void AddSlicedTriangle(int i1, Vector3 v2, Vector2 uv2, int i3) {
            int v1 = _vMapping[i1],
                v3 = _vMapping[i3];
            // 获得三角形的法线
            Vector3 normal = Vector3.Cross(v2 - vertices[v1], vertices[v3] - v2).normalized;

            triangles.Add(v1);
            AddPoint(v2, normal, uv2);
            triangles.Add(v3);

            //Compute triangle area
            surfacearea += GetTriangleArea(triangles.Count - 3);
        }

        public void AddSlicedTriangle(int i1, Vector3 v2, Vector3 v3, Vector2 uv2, Vector2 uv3) {
            // Compute face normal?
            int v1 = _vMapping[i1];
            Vector3 normal = Vector3.Cross(v2 - vertices[v1], v3 - v2).normalized;

            triangles.Add(v1);
            AddPoint(v2, normal, uv2);
            AddPoint(v3, normal, uv3);

            //Compute triangle area
            surfacearea += GetTriangleArea(triangles.Count - 3);
        }

        public void AddNewTriangle(Vector3[] points) {
            Vector3 normal = Vector3.Cross(points[1] - points[0], points[2] - points[1]).normalized;
            for (int i = 0; i < 3; ++i) {
                AddPoint(points[i], normal, Vector2.zero);
            }
            surfacearea += GetTriangleArea(triangles.Count - 3);
        }

        public void ContainsKeys(List<int> triangles, int startIdx, bool[] isTrue) {
            for (int i = 0; i < 3; ++i)
                isTrue[i] = _vMapping.ContainsKey(triangles[startIdx + i]);
        }

        public void AddVertex(List<Vector3> ogVertices, List<Vector3> ogNormals, List<Vector2> ogUvs, int index) {
            _vMapping[index] = vertices.Count;
            vertices.Add(ogVertices[index]);
            normals.Add(ogNormals[index]);
            uvs.Add(ogUvs[index]);
        }

        //Compute triangle area
        private float GetTriangleArea(int i) {
            var va = vertices[triangles[i + 2]] - vertices[triangles[i]];
            var vb = vertices[triangles[i + 1]] - vertices[triangles[i]];
            float a = va.magnitude;
            float b = vb.magnitude;
            float gamma = Mathf.Deg2Rad * Vector3.Angle(vb, va);
            return a * b * Mathf.Sin(gamma) / 2;
        }
    }
}