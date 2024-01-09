using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MinecraftVoxelTerrain {

    public static class Tables {
        // all 8 possible vertices for a voxel
        public static readonly Vector3[] Vertices = new Vector3[8] {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
        };

        // vertices to build a quad for each side of a voxel
        public static readonly int[,] QuadVertices = new int[6, 4] {
            // quad order
            // right, left, up, down, front, back

            // 0 1 2 2 1 3 <- triangle numbers

            // quads
            {1, 2, 5, 6}, // right quad
            {4, 7, 0, 3}, // left quad

            {3, 7, 2, 6}, // up quad
            {1, 5, 0, 4}, // down quad

            {5, 6, 4, 7}, // front quad
            {0, 3, 1, 2}, // back quad
        };

        // ÌåËØµÄÁÚ¾Ó
        // offset to neighboring voxel position
        public static readonly int3[] NeighborOffsets = new int3[6] {
            new int3( 1,  0,  0), // right
            new int3(-1,  0,  0), // left
            new int3( 0,  1,  0), // up
            new int3( 0, -1,  0), // down
            new int3( 0,  0,  1), // front
            new int3( 0,  0, -1), // back
        };
    }
}