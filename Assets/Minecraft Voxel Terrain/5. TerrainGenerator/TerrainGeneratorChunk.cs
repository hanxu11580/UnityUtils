using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


// https://sparker3d.com/paper/minecraft-unity/16

namespace MinecraftVoxelTerrain {
    //[ExecuteInEditMode]
    public class TerrainGeneratorChunk : MonoBehaviour {

        [SerializeField] TerrainGenerator _terrainGenerator;

        private void Start() {
            // 初始化TerrainGenerator
            _terrainGenerator.Init();

            // 在ChunkResolution这个范围内生成体素
            for (int x = 0; x < _terrainGenerator.ChunkResolution; x++) {
                for (int y = 0; y < _terrainGenerator.ChunkResolution; y++) {
                    for (int z = 0; z < _terrainGenerator.ChunkResolution; z++) {
                        // 根据随机高度控制，体素是否画出来，达到地形效果
                        if (IsSolid(x, y, z)) {
                            MeshVoxel(x, y, z);
                        }
                    }
                }
            }

            // Complete TerrainGenerator
            _terrainGenerator.Complete();
        }

        private void MeshVoxel(int x, int y, int z) {
            Vector3 offsetPos = new Vector3(x, y, z);
            var vexelType = _terrainGenerator.GetVoxelType(x, y, z);
            for (int side = 0; side < 6; side++) {
                // 如果是实心的跳过
                if (IsNeighborSolid(x, y, z, side, vexelType)) {
                    continue;
                }
                _terrainGenerator.Layers[vexelType.layer].MeshQuad(offsetPos, vexelType, side);
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
            if (x < 0 || x >= _terrainGenerator.ChunkResolution
                || y < 0 || y >= _terrainGenerator.ChunkResolution
                || z < 0 || z >= _terrainGenerator.ChunkResolution) {
                return false;
            }
            return _terrainGenerator.GetVoxelType(x, y, z).isSolid;
        }

        private bool IsNeighborSolid(int x, int y, int z, int side, LayerVoxelType selfVoxelType) {
            int3 neighborOffset = Tables.NeighborOffsets[side];
            var neighborPos = new Vector3Int(x + neighborOffset.x, y + neighborOffset.y, z + neighborOffset.z);
            var neighborVoxelType = _terrainGenerator.GetVoxelType(neighborPos.x, neighborPos.y, neighborPos.z);
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