using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


// https://sparker3d.com/paper/minecraft-unity/14

namespace MinecraftVoxelTerrain {
    //[ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour {
        [SerializeField] private int ChunkResolution = 16;
        [SerializeField] private VoxelType[] _voxelTypes;
        [SerializeField] private int _atlasSize = 1;
        private Vector3[] _vertices;
        private int[] _triangles;
        private Vector2[] _uvs;

        private int vertexOffset = 0;
        private int triangleOffset = 0;

        private FastNoiseLite _fastNoiseLite;
        [SerializeField] private GameObject _debugPrefab;
        [SerializeField] private int _seed;
        private void Start() {
            _fastNoiseLite = new FastNoiseLite(_seed);

            // һ��������Ҫ24�������36��������������
            _vertices = new Vector3[24 * ChunkResolution * ChunkResolution * ChunkResolution];
            _uvs = new Vector2[24 * ChunkResolution * ChunkResolution * ChunkResolution];
            _triangles = new int[36 * ChunkResolution * ChunkResolution * ChunkResolution];
            // ��ChunkResolution�����Χ����������
            for (int x = 0; x < ChunkResolution; x++) {
                for (int y = 0; y < ChunkResolution; y++) {
                    for (int z = 0; z < ChunkResolution; z++) {
                        // ��������߶ȿ��ƣ������Ƿ񻭳������ﵽ����Ч��
                        // ������z�ử��Ȼ�������y�ᣬ���������x�ᣨ�����ƶ���
                        if (IsSolid(x, y, z)) {
                            MeshVoxel(x, y, z);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            // ���������������� 16 λ��һ�����������֧�� 65535 �����㣩
            // 32 λ�����֧�� 40 �ڸ����㣩
            // �������������ʾ��ȷ��ɵĴ󷽿�
            // �������һ���������ķ���

            // һ���Ż��������Ȼ����ʹ��16λ�������豸��֧��32λ
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = _vertices;
            mesh.triangles = _triangles;
            mesh.uv = _uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            var filter = gameObject.GetComponent<MeshFilter>();
            filter.mesh = mesh;
        }

        private void MeshVoxel(int x, int y, int z) {
            Vector3 offsetPos = new Vector3(x, y, z);
            for (int side = 0; side < 6; side++) {

                if(x == 4 && y==8 && z==15 && side == 0) {
                    Debug.Log("");
                }


                // �����ʵ�ĵ�����
                if (IsNeighborSolid(x, y, z, side)) {
                    continue;
                }

                _vertices[vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]] + offsetPos;
                _vertices[vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]] + offsetPos;
                _vertices[vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]] + offsetPos;
                _vertices[vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]] + offsetPos;

                var g1 = GameObject.Instantiate(_debugPrefab, _vertices[vertexOffset + 0], Quaternion.identity);
                g1.name = (vertexOffset + 0).ToString();
                var g2 = GameObject.Instantiate(_debugPrefab, _vertices[vertexOffset + 1], Quaternion.identity);
                g2.name = (vertexOffset + 1).ToString();
                var g3 = GameObject.Instantiate(_debugPrefab, _vertices[vertexOffset + 2], Quaternion.identity);
                g3.name = (vertexOffset + 2).ToString();
                var g4 = GameObject.Instantiate(_debugPrefab, _vertices[vertexOffset + 3], Quaternion.identity);
                g4.name = (vertexOffset + 3).ToString();

                _triangles[triangleOffset + 0] = vertexOffset + 0;
                _triangles[triangleOffset + 1] = vertexOffset + 1;
                _triangles[triangleOffset + 2] = vertexOffset + 2;
                _triangles[triangleOffset + 3] = vertexOffset + 2;
                _triangles[triangleOffset + 4] = vertexOffset + 1;
                _triangles[triangleOffset + 5] = vertexOffset + 3;

                // ע�͵���ʹ��Assets/Minecraft Voxel Terrain/Textures/Texture.png��Ϊ������ͼ
                //_uvs[vertexOffset + 0] = new Vector2(0, 0);
                //_uvs[vertexOffset + 1] = new Vector2(0, 1);
                //_uvs[vertexOffset + 2] = new Vector2(1, 0);
                //_uvs[vertexOffset + 3] = new Vector2(1, 1);
                var voxelType = GetVoxelType(x, y, z);
                var atlasOffset = voxelType.GetAtlasOffset(side);
                _uvs[vertexOffset + 0] = (atlasOffset + new Vector2(0, 0)) / _atlasSize;
                _uvs[vertexOffset + 1] = (atlasOffset + new Vector2(0, 1)) / _atlasSize;
                _uvs[vertexOffset + 2] = (atlasOffset + new Vector2(1, 0)) / _atlasSize;
                _uvs[vertexOffset + 3] = (atlasOffset + new Vector2(1, 1)) / _atlasSize;

                triangleOffset += 6;
                vertexOffset += 4;
            }
        }

        /// <summary>
        /// �Ƿ���ʵ������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool IsSolid(int x, int y, int z) {
            // ������Χ�Ĳ���ȥ������
            // ��������˵���ھӳ����Ļ����Ὣ���滭����
            //if (x < 0 || x >= ChunkResolution
            //    || y < 0 || y >= ChunkResolution
            //    || z < 0 || z >= ChunkResolution) {
            //    return false;
            //}

            // û�е�������ʱ
            //float terrainHeight = GetNoiseHeight(8, 1, x, z);
            //if (y > terrainHeight) {
            //    return false;
            //}
            //return true;
            return GetVoxelType(x, y, z).isSolid;
        }

        private VoxelType GetVoxelType(int x, int y, int z) {
            float terrainHeight = GetNoiseHeight(8, 1, x, z);
            if (y == 5) {
                return _voxelTypes[3]; // ɳ��
            }

            if (y < terrainHeight) {
                return _voxelTypes[1]; // ����
            }
            // ������һ���ǲ�
            if (y < (terrainHeight + 1)) {
                return _voxelTypes[2]; // ��
            }
            else if (y < 8) {
                return _voxelTypes[4]; // ˮ
            }

            return _voxelTypes[0]; // ����
        }

        // ���floorHeight ~ maxHeight�߶�
        private float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z) {
            var n1 = _fastNoiseLite.GetNoise(x, z); // -1 ~ 1
            var n2 = n1 + 1; // 0 ~ 2
            var n3 = n2 / 2; // 0 ~ 1
            var n4 = n3 * (maxHeight - floorHeight); // 0 ~ maxHeight - floorHeight
            return n4 + floorHeight; // floorHeight ~ maxHeight
        }

        private bool IsNeighborSolid(int x, int y, int z, int side) {
            int3 neighborOffset = Tables.NeighborOffsets[side];
            return IsSolid(x + neighborOffset.x, y + neighborOffset.y, z + neighborOffset.z);
        }
    }
}