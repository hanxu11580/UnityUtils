using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    public abstract class TerrainGenerator : MonoBehaviour {
        public int ChunkResolution = 32;
        public int Seed = 1337;
        public LayerVoxelType[] VoxelTypes;
        public Layer[] Layers;

        protected FastNoiseLite _fastNoiseLite;

        public void Init() {
            _fastNoiseLite = new FastNoiseLite(Seed);
            for (int i = 0; i < Layers.Length; i++) {
                Layers[i].Init(ChunkResolution);
            }
        }

        public void Complete() {
            for (int i = 0; i < Layers.Length; i++) {
                Layers[i].Complete();
            }
        }

        public abstract LayerVoxelType GetVoxelType(int x, int y, int z);
    }
}