using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


// https://sparker3d.com/paper/minecraft-unity/16

namespace MinecraftVoxelTerrain {
    //[ExecuteInEditMode]
    public class LayerChunk : MonoBehaviour {
        [SerializeField] private int ChunkResolution = 16;
        [SerializeField] private LayerVoxelType[] _voxelTypes;
        [SerializeField] private Layer[] _layers;
        private FastNoiseLite _fastNoiseLite;
        [SerializeField] private GameObject _debugPrefab;

        private void Start() {
            _fastNoiseLite = new FastNoiseLite();

            // 初始化Layer
            for (int i = 0; i < _layers.Length; i++) {
                _layers[i].Init(ChunkResolution);
            }

            // 在ChunkResolution这个范围内生成体素
            for (int x = 0; x < ChunkResolution; x++) {
                for (int y = 0; y < ChunkResolution; y++) {
                    for (int z = 0; z < ChunkResolution; z++) {
                        // 根据随机高度控制，体素是否画出来，达到地形效果
                        if (IsSolid(x, y, z)) {
                            MeshVoxel(x, y, z);
                        }
                    }
                }
            }

            // Complete layers
            for (int i = 0; i < _layers.Length; i++) {
                _layers[i].Complete();
            }
        }

        private void MeshVoxel(int x, int y, int z) {
            Vector3 offsetPos = new Vector3(x, y, z);
            var vexelType = GetVoxelType(x, y, z);
            for (int side = 0; side < 6; side++) {
                // 如果是实心的跳过
                if (IsNeighborSolid(x, y, z, side, vexelType)) {
                    continue;
                }
                _layers[vexelType.layer].MeshQuad(offsetPos, vexelType, side);
            }
        }

        /// <summary>
        /// 是否是实心体素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool IsSolid(int x, int y, int z) {
            // 超过范围的不会去画体素
            // 对于面来说，邻居超过的话将会将此面画出来
            if (x < 0 || x >= ChunkResolution
                || y < 0 || y >= ChunkResolution
                || z < 0 || z >= ChunkResolution) {
                return false;
            }

            // 没有地形类型时
            //float terrainHeight = GetNoiseHeight(8, 1, x, z);
            //if (y > terrainHeight) {
            //    return false;
            //}
            //return true;
            return GetVoxelType(x, y, z).isSolid;
        }

        private LayerVoxelType GetVoxelType(int x, int y, int z) {
            float terrainHeight = GetNoiseHeight(8, 1, x, z);
            if (y == 5) {
                return _voxelTypes[3]; // 沙子
            }

            if (y < terrainHeight) {
                return _voxelTypes[1]; // 泥土
            }
            // 泥土上一层是草
            if (y < (terrainHeight + 1)) {
                return _voxelTypes[2]; // 草
            }
            else if (y < 8) {
                return _voxelTypes[4]; // 水
            }

            return _voxelTypes[0]; // 空气
        }

        // 获得 floorHeight ~ maxHeight高度
        private float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z) {
            var n1 = _fastNoiseLite.GetNoise(x, z); // -1 ~ 1
            var n2 = n1 + 1; // 0 ~ 2
            var n3 = n2 / 2; // 0 ~ 1
            var n4 = n3 * (maxHeight - floorHeight); // 0 ~ maxHeight - floorHeight
            return n4 + floorHeight; // floorHeight ~ maxHeight
        }

        private bool IsNeighborSolid(int x, int y, int z, int side, LayerVoxelType selfVoxelType) {
            int3 neighborOffset = Tables.NeighborOffsets[side];
            var neighborPos = new Vector3Int(x + neighborOffset.x, y + neighborOffset.y, z + neighborOffset.z);
            var neighborVoxelType = GetVoxelType(neighborPos.x, neighborPos.y, neighborPos.z);
            if(neighborVoxelType.layer != selfVoxelType.layer) {
                // 如果块与块之前层不一样，将会画出来
                // 目前只有水层和普通层2层，所以泥土、沙子、草之间的边界依旧不会画出来
                // 只有泥土、沙子、草和水的边界会画出边界面
                return false;
            }

            return IsSolid(neighborPos.x, neighborPos.y, neighborPos.z);
        }
    }
}