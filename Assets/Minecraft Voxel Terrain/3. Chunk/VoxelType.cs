using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    [System.Serializable]
    public class VoxelType {
        public string name = "default";
        public bool isSolid = false;
        [SerializeField]
        private Vector2 _atlasOffsetTop = Vector2.zero;
        [SerializeField]
        private Vector2 _atlasOffsetSides = Vector2.zero;
        [SerializeField]
        private Vector2 _atlasOffsetBottom = Vector2.zero;

        // 举例草
        // _atlasOffsetTop = (1,0)
        // _atlasOffsetSides = (0,0)
        // _atlasOffsetBottom = (0,0)
        // 这样可以形成顶面是草，侧面是泥土的方块
        public Vector2 GetAtlasOffset(int side) {
            // top
            if (side == 2) {
                return _atlasOffsetTop;
            }
            // bottom
            else if (side == 3) {
                return _atlasOffsetBottom;
            }

            return _atlasOffsetSides;
        }
    }
}