using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Voxel : MonoBehaviour {

        void Start() {
            const int quadsNum = 6;
            // 不考虑共享顶点有24个顶点
            Vector3[] vertices = new Vector3[4 * quadsNum];
            Vector2[] uvs = new Vector2[4 * quadsNum];
            // 一个面有2个三角形6个顶点
            int[] triangles = new int[6 * quadsNum];

            int vertexOffset = 0;
            int triangleOffset = 0;

            for (int side = 0; side < quadsNum; side++) {
                // 每个面4个顶点
                vertices[vertexOffset + 0] = Tables.Vertices[Tables.QuadVertices[side, 0]];
                vertices[vertexOffset + 1] = Tables.Vertices[Tables.QuadVertices[side, 1]];
                vertices[vertexOffset + 2] = Tables.Vertices[Tables.QuadVertices[side, 2]];
                vertices[vertexOffset + 3] = Tables.Vertices[Tables.QuadVertices[side, 3]];
                // 三角形
                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + 2;
                triangles[triangleOffset + 3] = vertexOffset + 2;
                triangles[triangleOffset + 4] = vertexOffset + 1;
                triangles[triangleOffset + 5] = vertexOffset + 3;
                // uvs
                uvs[vertexOffset + 0] = new Vector2(0, 0);
                uvs[vertexOffset + 1] = new Vector2(0, 1);
                uvs[vertexOffset + 2] = new Vector2(1, 0);
                uvs[vertexOffset + 3] = new Vector2(1, 1);


                vertexOffset += 4;
                triangleOffset += 6;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter mFilter = GetComponent<MeshFilter>();
            mFilter.mesh = mesh;
        }
    }
}