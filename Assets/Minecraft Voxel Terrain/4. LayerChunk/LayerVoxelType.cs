using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    [System.Serializable]
    public class LayerVoxelType {
        public string name = "default";
        public bool isSolid = false;
        public Vector2 atlasOffset = Vector2.zero;
        public int layer = 0; // add this
    }
}
