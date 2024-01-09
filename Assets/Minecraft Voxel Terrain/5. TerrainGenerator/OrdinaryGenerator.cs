using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    public class OrdinaryGenerator : TerrainGenerator {

        public override LayerVoxelType GetVoxelType(int x, int y, int z) {
            float terrainHeight = GetNoiseHeight(8, 1, x, z);
            if (y == 5) {
                return VoxelTypes[3]; // 沙子
            }

            if (y < terrainHeight) {
                return VoxelTypes[1]; // 泥土
            }
            // 泥土上一层是草
            if (y < (terrainHeight + 1)) {
                return VoxelTypes[2]; // 草
            }
            else if (y < 8) {
                return VoxelTypes[4]; // 水
            }

            return VoxelTypes[0]; // 空气
        }

        // 获得 floorHeight ~ maxHeight高度
        private float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z) {
            var n1 = _fastNoiseLite.GetNoise(x, z); // -1 ~ 1
            var n2 = n1 + 1; // 0 ~ 2
            var n3 = n2 / 2; // 0 ~ 1
            var n4 = n3 * (maxHeight - floorHeight); // 0 ~ maxHeight - floorHeight
            return n4 + floorHeight; // floorHeight ~ maxHeight
        }
    }
}
