using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {

    [Serializable]
    public struct WorldVoxelType {
        public string name;
        public bool isSolid;
        public Vector2 topAtlasOffset;
        public Vector2 atlasOffset;
        public Vector2 bottomAtlasOffset;
    }


    [Serializable]
    public struct JobWorldVoxelType {
        public bool isSolid;
        public Vector2 topAtlasOffset;
        public Vector2 atlasOffset;
        public Vector2 bottomAtlasOffset;

        // ������
        // _atlasOffsetTop = (1,0)
        // _atlasOffsetSides = (0,0)
        // _atlasOffsetBottom = (0,0)
        // ���������γɶ����ǲݣ������������ķ���
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
