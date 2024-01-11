using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {

    [Serializable]
    public struct DynamicWorldVoxelType {
        public string name;
        public bool isSolid;
        public Vector2 topAtlasOffset;
        public Vector2 atlasOffset;
        public Vector2 bottomAtlasOffset;
    }


    [Serializable]
    public struct JobDynamicWorldVoxelType {
        public bool isSolid;
        public Vector2 topAtlasOffset;
        public Vector2 atlasOffset;
        public Vector2 bottomAtlasOffset;

        // 举例草
        // _atlasOffsetTop = (1,0)
        // _atlasOffsetSides = (0,0)
        // _atlasOffsetBottom = (0,0)
        // 这样可以形成顶面是草，侧面是泥土的方块
        public Vector2 GetAtlasOffset(int side) {
            // top
            if (side == 2) {
                return topAtlasOffset;
            }
            // bottom
            else if (side == 3) {
                return bottomAtlasOffset;
            }

            return atlasOffset;
        }
    }

}
