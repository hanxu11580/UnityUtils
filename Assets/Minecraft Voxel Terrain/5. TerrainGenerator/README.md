- 现在如果想要更换地形将要修改GetVoxelType等代码才可以生成新地形，这节将会使代码复用达到生成各种各样的地形

- 首先先观察代码，如果要创建新地形，哪些代码需要变动，哪些代码不需要变动？
	+ 体素类型
	+ 层
	+ 方法 GetVoxelType_ (目前我只观察到这个，上面2个我感觉不需要变，下面再看)
	
- 第一次优化 TerrainGenerator和TerrainGeneratorChunk
	+ 下面2个代码只能生成浮空岛，所以可以将TerrainGenerator的GetVoxelType方法抽象，每个地形生成器都继承TerrainGenerator
```csharp
    public class TerrainGenerator : MonoBehaviour {
        public int ChunkResolution = 32;
        public int Seed = 1337;
        public LayerVoxelType[] VoxelTypes;
        public Layer[] Layers;

        private FastNoiseLite _fastNoiseLite;

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

        public LayerVoxelType GetVoxelType(int x, int y, int z) {
            float caves = GetNoiseCaves(5f, x, y, z);

            if (caves > 0.3) {
                return VoxelTypes[0]; // 空气
            }

            return VoxelTypes[1]; // 泥巴
        }

        // 获得 floorHeight ~ maxHeight高度
        private float GetNoiseHeight(float maxHeight, float floorHeight, float x, float z) {
            var n1 = _fastNoiseLite.GetNoise(x, z); // -1 ~ 1
            var n2 = n1 + 1; // 0 ~ 2
            var n3 = n2 / 2; // 0 ~ 1
            var n4 = n3 * (maxHeight - floorHeight); // 0 ~ maxHeight - floorHeight
            return n4 + floorHeight; // floorHeight ~ maxHeight
        }

        // 返回0 ~ 1
        public float GetNoiseCaves(float scale, int x, int y, int z) {
            return (_fastNoiseLite.GetNoise(x * scale, y * scale, z * scale) + 1) / 2;
        }
    }
```

```csharp
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
```
