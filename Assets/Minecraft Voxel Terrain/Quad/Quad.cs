using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftVoxelTerrain {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Quad : MonoBehaviour {

        /*
         *                          | 
         *                          | 
         *                          | 
         *              1           |             3
         *                          | 
         *                          |  
         *                          |    
         *                          |      
         *       ___________________|___________________
         *                          | 
         *                          | 
         *                          | 
         *                          | 
         *                          | 
         *              0           |             2
         *                          |  
         *                          |    
         *                          |
         *       
         */
        void Start() {
            var mf = GetComponent<MeshFilter>();
            var mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(-1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(1, 1, 0),
            };
            mesh.uv = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,0),
                new Vector2(1,1),
            };
            mesh.triangles = new int[] {
                0,1,2,
                1,3,2
            };
            mesh.RecalculateNormals();
            mf.mesh = mesh;
        }
    }
}