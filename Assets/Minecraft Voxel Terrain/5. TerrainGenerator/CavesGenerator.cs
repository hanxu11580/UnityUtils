using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    public class CavesGenerator : TerrainGenerator {
        public override LayerVoxelType GetVoxelType(int x, int y, int z) {
            float caves = GetNoiseCaves(5f, x, y, z);
            if (caves < 0.3) {
                return VoxelTypes[0]; // ¿ÕÆø
            }
            return VoxelTypes[1]; // Äà°Í
        }

        private float GetNoiseCaves(float scale, int x, int y, int z) {
            return (_fastNoiseLite.GetNoise(x * scale, y * scale, z * scale) + 1) / 2;
        }
    }
}