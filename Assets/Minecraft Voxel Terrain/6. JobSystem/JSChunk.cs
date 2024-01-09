using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// https://sparker3d.com/paper/minecraft-terrain-unity-jobs/6


namespace MinecraftVoxelTerrain {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class JSChunk : MonoBehaviour {

        public int chunkResolution = 16;

        private NativeArray<Vector3> _vertices;
        private NativeArray<int> _triangles;
        private JobHandle _jobHandle;

        private MeshFilter _meshFilter;

        private void Start() {
            _meshFilter = GetComponent<MeshFilter>();
            Schedule();
            Complete();
        }

        public void Schedule() {
            _vertices = new NativeArray<Vector3>(24 * chunkResolution * chunkResolution * chunkResolution, Allocator.TempJob);
            _triangles = new NativeArray<int>(36 * chunkResolution * chunkResolution * chunkResolution, Allocator.TempJob);
            var job = new MeshJob() {
                vertices = _vertices,
                triangles = _triangles,
                chunkResolution = chunkResolution
            };
            _jobHandle = job.Schedule();
        }

        public void Complete() {
            _jobHandle.Complete();

            var mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
            _vertices.Dispose();
            _triangles.Dispose();
        }
    }
}