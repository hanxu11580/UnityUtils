using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes_CSharp {

    public struct Triangle {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;

        public static int SizeOf => sizeof(float) * 3 * 3;
    }

    public class Chunk : MonoBehaviour {
        public NoiseGenerator noiseGenerator;
        public float isoLevel = 0.5f;
        public Material material;

        private MarchingCubesCompute marchingCubesCompute;

        private MeshFilter _mFilter;
        private MeshRenderer _mRenderer;
        private MeshCollider _mCollider;

        private float[] _weights;
        private Mesh _mesh;

        private void Start() {
            _mFilter = GetComponent<MeshFilter>();
            _mCollider = GetComponent<MeshCollider>();
            _mRenderer = GetComponent<MeshRenderer>();

            _mesh = new Mesh();
            _weights = noiseGenerator.GetNoise();
            marchingCubesCompute = new MarchingCubesCompute();
            _mRenderer.material = material;

            UpdateMesh();
        }

        private Mesh ConstructMesh() {
            marchingCubesCompute.Weights = _weights;
            marchingCubesCompute.IsoLevel = isoLevel;
            marchingCubesCompute.March();

            Triangle[] triangles = marchingCubesCompute.GetTriangles();
            return CreateMeshFromTriangles(triangles);
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

            _mesh.Clear();
            _mesh.vertices = verts;
            _mesh.triangles = tris;
            _mesh.RecalculateNormals();
            return _mesh;
        }

        private void UpdateMesh() {
            Mesh mesh = ConstructMesh();
            _mFilter.sharedMesh = mesh;
            _mCollider.sharedMesh = mesh;
        }
    }
}