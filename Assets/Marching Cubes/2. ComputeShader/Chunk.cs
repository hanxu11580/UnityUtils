using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MarchingCubes_ComputeShader {
    public class Chunk : MonoBehaviour {

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;

        struct Triangle {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;

            public static int SizeOf => sizeof(float) * 3 * 3;
        }


        public NoiseGenerator noiseGenerator;
        public ComputeShader marchingShader;
        [Range(0, 7)]
        public int lod;

        private float[] _weights;
        private ComputeBuffer _trianglesBuffer;
        private ComputeBuffer _trianglesCountBuffer;
        private ComputeBuffer _weightsBuffer;
        private Mesh _mesh;

        private void Start() {
            Create();
        }

        private void OnValidate() {
            if (Application.isPlaying) {
                Create();
            }
        }

        public void Create() {
            CreateBuffers();
            // 确保噪音只生成一次
            if(_weights == null) {
                _weights = noiseGenerator.GetNoise(GridMetrics.LastLod);
            }
            _mesh = new Mesh();
            UpdateMesh();
            ReleaseBuffers();
        }

        private int ReadTriangleCount() {
            // _trianglesBuffer.count是最大容量，而不是实际值
            ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);

            int[] triCount = { 0 };
            _trianglesCountBuffer.GetData(triCount);
            return triCount[0];
        }

        private Mesh ConstructMesh() {
            int kernel = marchingShader.FindKernel("March");

            marchingShader.SetBuffer(kernel, "_Triangles", _trianglesBuffer);
            marchingShader.SetBuffer(kernel, "_Weights", _weightsBuffer);
            //marchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk(lod));
            marchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk(GridMetrics.LastLod));
            marchingShader.SetInt("_LODSize", GridMetrics.PointsPerChunk(lod));
            // 例如：使用 LOD 0（每块点数 = 8）时，我们必须相对于最后一个 LOD（每块点数 = 40）缩放 3D 网格内的所有点。这意味着点 (1,1,1) 实际上必须代表点 (5,5,5)。为了得到这个新点，我们使用以下公式：
            // Scale factor = Chunk size / lod size
            // New point = original point* scale factor
            float lodScaleFactor = ((float)GridMetrics.PointsPerChunk(GridMetrics.LastLod)) / (float)GridMetrics.PointsPerChunk(lod);

            marchingShader.SetFloat("_LodScaleFactor", lodScaleFactor);
            marchingShader.SetInt("_Scale", GridMetrics.Scale);
            marchingShader.SetFloat("_IsoLevel", .5f);

            _weightsBuffer.SetData(_weights);
            _trianglesBuffer.SetCounterValue(0);

            marchingShader.Dispatch(
                kernel, GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod));

            Triangle[] triangles = new Triangle[ReadTriangleCount()];
            _trianglesBuffer.GetData(triangles);
            return CreateMeshFromTriangles(triangles);
        }

        private void CreateBuffers() {
            // (GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk) 立方体个数
            // 每个立方体配置最多可以有 5 个三角形
            // ComputeBufferType.Append 类似列表追加Add
            _trianglesBuffer = new ComputeBuffer(5 * (GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod)), Triangle.SizeOf, ComputeBufferType.Append);
            _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            //_weightsBuffer = new ComputeBuffer(GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod), sizeof(float));
            // 返回固定数量的噪音值
            _weightsBuffer = new ComputeBuffer(GridMetrics.PointsPerChunk(GridMetrics.LastLod) * GridMetrics.PointsPerChunk(GridMetrics.LastLod) * GridMetrics.PointsPerChunk(GridMetrics.LastLod), sizeof(float));
        }

        private void ReleaseBuffers() {
            _trianglesBuffer.Release();
            _trianglesCountBuffer.Release();
            _weightsBuffer.Release();
        }

        private Mesh CreateMeshFromTriangles(Triangle[] triangles) {
            Vector3[] verts = new Vector3[triangles.Length * 3];
            int[] tris = new int[triangles.Length * 3];

            for (int i = 0; i < triangles.Length; i++) {
                int startIndex = i * 3;
                verts[startIndex] = triangles[i].a;
                verts[startIndex + 1] = triangles[i].b;
                verts[startIndex + 2] = triangles[i].c;
                tris[startIndex] = startIndex;
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
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        /// <summary>
        /// 修改weights 用UpdateWeights compute shader函数
        /// </summary>
        /// <param name="hitPosition"></param>
        /// <param name="brushSize"></param>
        /// <param name="add"></param>
        public void EditWeights(Vector3 hitPosition, float brushSize, bool add) {
            CreateBuffers();
            // 根据核心函数名找到核心index
            int kernel = marchingShader.FindKernel("UpdateWeights");

            _weightsBuffer.SetData(_weights);
            marchingShader.SetBuffer(kernel, "_Weights", _weightsBuffer);

            //marchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk(lod));
            marchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk(GridMetrics.LastLod));
            marchingShader.SetVector("_HitPosition", hitPosition);
            marchingShader.SetFloat("_BrushSize", brushSize);

            marchingShader.SetFloat("_TerraformStrength", add ? 1f : -1f);
            marchingShader.SetInt("_Scale", GridMetrics.Scale);
            marchingShader.Dispatch(kernel, GridMetrics.ThreadGroups(GridMetrics.LastLod), GridMetrics.ThreadGroups(GridMetrics.LastLod), GridMetrics.ThreadGroups(GridMetrics.LastLod));

            _weightsBuffer.GetData(_weights);

            UpdateMesh();
            ReleaseBuffers();
        }
    }
}