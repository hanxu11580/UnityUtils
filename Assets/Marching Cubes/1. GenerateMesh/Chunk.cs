using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes {

    public struct Triangle {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;

        public static int SizeOf => sizeof(float) * 3 * 3;
    }

    public class Chunk : MonoBehaviour {
        public bool drawGizmos;
        public NoiseGenerator noiseGenerator;
        public float isoLevel = 0.5f;
        public Material material;

        private MarchingCubesCompute marchingCubesCompute;

        private MeshFilter _mFilter;
        private MeshRenderer _mRenderer;

        float[] _weights;

        private void Start() {
            _mFilter = GetComponent<MeshFilter>();
            _mRenderer = GetComponent<MeshRenderer>();

            _weights = noiseGenerator.GetNoise();
            marchingCubesCompute = new MarchingCubesCompute();
            marchingCubesCompute.Weights = _weights;
            marchingCubesCompute.IsoLevel = isoLevel;
            marchingCubesCompute.March();

            Triangle[] triangles = marchingCubesCompute.GetTriangles();
            _mFilter.sharedMesh = CreateMeshFromTriangles(triangles);
            _mRenderer.material = material;
        }

        private void OnDrawGizmos() {
            if (!drawGizmos) {
                return;
            }

            if (_weights == null || _weights.Length == 0) {
                return;
            }
            for (int x = 0; x < GridMetrics.PointsPerChunk; x++) {
                for (int y = 0; y < GridMetrics.PointsPerChunk; y++) {
                    for (int z = 0; z < GridMetrics.PointsPerChunk; z++) {
                        // 立方体
                        // 下面这个index公式是因为在ComputeShader中下面这样计算的，反过来可以可以获得权重值索引
                        // _Weights[indexFromCoord(id.x, id.y, id.z)] = n;
                        int index = x + GridMetrics.PointsPerChunk * (y + GridMetrics.PointsPerChunk * z);

                        float noiseValue = _weights[index];
                        // 权重越高越白，反之越低越黑
                        Gizmos.color = Color.Lerp(Color.black, Color.white, noiseValue);
                        Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * .2f);
                    }
                }
            }
        }

        private Mesh CreateMeshFromTriangles(Triangle[] triangles) {
            // 顶点
            Vector3[] verts = new Vector3[triangles.Length * 3];
            // 三角面索引
            int[] tris = new int[triangles.Length * 3];

            for (int i = 0; i < triangles.Length; i++) {
                // 0, 3, 6
                int startIndex = i * 3;
                verts[startIndex + 0] = triangles[i].v1;
                verts[startIndex + 1] = triangles[i].v2;
                verts[startIndex + 2] = triangles[i].v3;

                // triangles[i].v1在verts的索引
                tris[startIndex + 0] = startIndex + 0;
                tris[startIndex + 1] = startIndex + 1;
                tris[startIndex + 2] = startIndex + 2;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            return mesh;
        }

        [Button("Rebuild")]
        private void Reuild() {
            _weights = noiseGenerator.GetNoise();
            marchingCubesCompute.Weights = _weights;
            marchingCubesCompute.IsoLevel = isoLevel;
            marchingCubesCompute.March();

            Triangle[] triangles = marchingCubesCompute.GetTriangles();
            _mFilter.sharedMesh = CreateMeshFromTriangles(triangles);
        }
    }
}