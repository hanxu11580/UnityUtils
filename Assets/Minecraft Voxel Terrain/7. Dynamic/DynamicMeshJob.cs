using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace MinecraftVoxelTerrain {
    /// <summary>
    /// ����������vertices��triangles�������
    /// </summary>
    public struct DynamicMeshJob : IJob {
        public Vector3 chunkPosition;
        public int chunkResolution;

        [ReadOnly]
        public NativeArray<JobDynamicWorldVoxelType> voxelTypes;
        [WriteOnly]
        public NativeArray<Vector3> vertices;
        [WriteOnly]
        public NativeArray<int> triangles;
        [WriteOnly]
        public NativeArray<Vector3> normals;
        [WriteOnly] public NativeArray<Vector2> uvs;
        public int atlasSize;

        private int _vertexIndex;
        private int _triangleIndex;


        public void Execute() {
            _vertexIndex = 0;
            _triangleIndex = 0;

            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            for (int x = 0; x < chunkResolution; x++) {
                for (int y = 0; y < chunkResolution; y++) {
                    for (int z = 0; z < chunkResolution; z++) {
                        if (IsSolid(fastNoiseLite, x, y, z)) {
                            MeshVoxel(fastNoiseLite, x, y, z);
                        }
                    }
                }
            }
        }

        private bool IsSolid(FastNoiseLite fastNoiseLite, int x, int y, int z) {
            return VoxelAt(fastNoiseLite, new Vector3Int(x, y, z)).isSolid;
        }

        private JobDynamicWorldVoxelType VoxelAt(FastNoiseLite noise, Vector3Int voxel) {
            float height = GetNoiseHeight(noise, voxel.x, voxel.z);

            // if below ground
            if (voxel.y < height) {
                return voxelTypes[1];
            }

            return voxelTypes[0];
        }

        /// <summary>
        /// var v1 = fastNoiseLite.GetNoise(chunkPosition.x + x, chunkPosition.z + z);
        /// ������������Խ�����������
        /// ����ChunkResolution = 1��GridResolution = 2
        /// ������������귶ΧΪ:(0,0,0)~(15,15,15) ��Ϊx,z
        /// 
        /// ��һ���chunkPosition.x��chunkPosition.z = 0
        /// ������x��z���꣺(x, z)
        /// 
        /// �ڶ����chunkPosition.x = 0��chunkPosition.z = 1
        /// ������x��z���꣺(x, z + 1)
        /// 
        /// �������chunkPosition.x = 1��chunkPosition.z = 0
        /// ������x��z���꣺(x + 1, z)
        /// 
        /// ���Ŀ��chunkPosition.x = 1��chunkPosition.z = 1
        /// ������x��z���꣺(x + 1, z + 1)
        /// 
        /// ��������������������
        /// 
        /// </summary>
        /// <param name="fastNoiseLite"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private float GetNoiseHeight(FastNoiseLite fastNoiseLite, int x, int z) {
            // [-1, 1]
            var v1 = fastNoiseLite.GetNoise(chunkPosition.x + x, chunkPosition.z + z);
            // [0, 2]
            var v2 = v1 + 1;
            // [0, 1]
            var v3 = v2 / 2;
            // [0, chunkResolution]
            var height = v3 * chunkResolution;
            return height;
        }

        private bool IsNeighborSolid(FastNoiseLite fastNoiseLite, int x,int y, int z, int side) {
            return IsSolid(
                fastNoiseLite,
                x + Tables.NeighborOffsets[side].x,
                y + Tables.NeighborOffsets[side].y,
                z + Tables.NeighborOffsets[side].z);
        }

        public void MeshVoxel(FastNoiseLite fastNoiseLite, int x, int y, int z) {
            // the position of the voxel
            // ��߻���x,zȥ����
            Vector3Int voxelPosition = new Vector3Int(x, y, z);
            var voxelType = VoxelAt(fastNoiseLite, voxelPosition);
            for (int side = 0; side < 6; side++) {
                if (!IsNeighborSolid(fastNoiseLite, x, y, z, side)) {
                    vertices[_vertexIndex + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]] + voxelPosition;
                    vertices[_vertexIndex + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]] + voxelPosition;
                    vertices[_vertexIndex + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]] + voxelPosition;
                    vertices[_vertexIndex + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]] + voxelPosition;

                    // normals
                    normals[_vertexIndex + 0] = Tables.Normals[side];
                    normals[_vertexIndex + 1] = Tables.Normals[side];
                    normals[_vertexIndex + 2] = Tables.Normals[side];
                    normals[_vertexIndex + 3] = Tables.Normals[side];

                    var realAtlasOffset = voxelType.GetAtlasOffset(side);
                    uvs[_vertexIndex + 0] = (realAtlasOffset + new Vector2(0, 0)) / atlasSize;
                    uvs[_vertexIndex + 1] = (realAtlasOffset + new Vector2(0, 1)) / atlasSize;
                    uvs[_vertexIndex + 2] = (realAtlasOffset + new Vector2(1, 0)) / atlasSize;
                    uvs[_vertexIndex + 3] = (realAtlasOffset + new Vector2(1, 1)) / atlasSize;

                    // 0 1 2 2 1 3 <- triangle index order
                    triangles[_triangleIndex + 0] = 0 + _vertexIndex;
                    triangles[_triangleIndex + 1] = 1 + _vertexIndex;
                    triangles[_triangleIndex + 2] = 2 + _vertexIndex;
                    triangles[_triangleIndex + 3] = 2 + _vertexIndex;
                    triangles[_triangleIndex + 4] = 1 + _vertexIndex;
                    triangles[_triangleIndex + 5] = 3 + _vertexIndex;

                    _vertexIndex += 4;
                    _triangleIndex += 6;
                }
            }
        }
    }
}