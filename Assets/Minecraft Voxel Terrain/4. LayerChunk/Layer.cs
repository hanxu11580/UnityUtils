using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    [System.Serializable]
    public class Layer {
        public string name = "default";
        public bool useCollider = true;
        public Material material;
        public int atlasSize = 1;

        private GameObject _gameObject;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;

        private int _vertexOffset = 0;
        private int _triangleOffset = 0;

        // 类似原来Chunk中Start里面的功能
        public void Init(int chunkResolution) {
            _gameObject = new GameObject();
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = material;

            if (useCollider) {
                _meshCollider = _gameObject.AddComponent<MeshCollider>();
            }
            _vertices = new Vector3[24 * chunkResolution * chunkResolution * chunkResolution];
            _triangles = new int[36 * chunkResolution * chunkResolution * chunkResolution];
            _uvs = new Vector2[24 * chunkResolution * chunkResolution * chunkResolution];
        }

        public void MeshQuad(Vector3 offsetPos, LayerVoxelType voxelType, int side) {
            _vertices[_vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]] + offsetPos;
            _vertices[_vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]] + offsetPos;
            _vertices[_vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]] + offsetPos;
            _vertices[_vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]] + offsetPos;

            _triangles[_triangleOffset + 0] = _vertexOffset + 0;
            _triangles[_triangleOffset + 1] = _vertexOffset + 1;
            _triangles[_triangleOffset + 2] = _vertexOffset + 2;
            _triangles[_triangleOffset + 3] = _vertexOffset + 2;
            _triangles[_triangleOffset + 4] = _vertexOffset + 1;
            _triangles[_triangleOffset + 5] = _vertexOffset + 3;

            // apply offset and add uv corner offsets
            // then divide by the atlas size to get a smaller section (AKA our individual texture)
            _uvs[_vertexOffset + 0] = (voxelType.atlasOffset + new Vector2(0, 0)) / atlasSize;
            _uvs[_vertexOffset + 1] = (voxelType.atlasOffset + new Vector2(0, 1)) / atlasSize;
            _uvs[_vertexOffset + 2] = (voxelType.atlasOffset + new Vector2(1, 0)) / atlasSize;
            _uvs[_vertexOffset + 3] = (voxelType.atlasOffset + new Vector2(1, 1)) / atlasSize;

            _triangleOffset += 6;
            _vertexOffset += 4;
        }

        public void Complete() {
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices;
            mesh.triangles = _triangles;
            mesh.uv = _uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            _meshFilter.mesh = mesh;

            if (useCollider) {
                _meshCollider.sharedMesh = mesh;
            }
        }
    }
}