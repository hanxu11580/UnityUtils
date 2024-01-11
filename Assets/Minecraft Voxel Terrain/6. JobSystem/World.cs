using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using USDT.Utils;

namespace MinecraftVoxelTerrain {

    public class WorldChunk {
        public bool IsScheduled { get; private set; }
        public bool IsCompleted => _jobHandle.IsCompleted;

        private JobHandle _jobHandle;
        private NativeArray<Vector3> _vertices;
        private NativeArray<int> _triangles;
        private NativeArray<Vector3> _normals;
        private NativeArray<Vector2> _uvs;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private GameObject _gameObject;
        private MeshCollider _meshCollider;

        private Vector3 _position;
        private World _world;

        public WorldChunk(World world, Vector3 pos) {
            // set references
            this._world = world;
            this._position = pos;

            _gameObject = new GameObject();
            _gameObject.transform.position = pos;
            _gameObject.transform.parent = world.transform;

            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = world.material;
            _meshCollider = _gameObject.AddComponent<MeshCollider>();
        }

        public void Schedule() {
            _vertices = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);
            _triangles = new NativeArray<int>(36 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);
            _normals = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);
            _uvs = new NativeArray<Vector2>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);

            var job = new MeshJob() {
                vertices = _vertices,
                triangles = _triangles,
                normals = _normals,
                uvs = _uvs,
                atlasSize = _world.atlasSize,
                chunkResolution = _world.chunkResolution,
                chunkPosition = _position,
                voxelTypes = _world.jobVoxelTypes,
            };
            _jobHandle = job.Schedule();
            IsScheduled = true;
        }

        public void Complete() {
            _jobHandle.Complete();

            var mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.normals = _normals.ToArray();
            mesh.SetUVs(0,_uvs);
            mesh.RecalculateBounds();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _vertices.Dispose();
            _triangles.Dispose();
            _normals.Dispose();
            _uvs.Dispose();

            IsScheduled = false;
        }

        public void Dispose() {
            _jobHandle.Complete();
            _vertices.Dispose();
            _triangles.Dispose();
            _normals.Dispose();
            _uvs.Dispose();
            IsScheduled = false;
        }
    }

    /// <summary>
    /// 效果就是直接生成一大块地区
    /// </summary>
    public class World : MonoBehaviour {
        public Material material;
        public int chunkResolution = 16;
        public int gridResolution = 8;
        public int atlasSize = 4;

        private WorldChunk[,] _chunks;
        private Stopwatch _stopwatch = new Stopwatch();


        [HideInInspector]
        public Bounds chunkBounds;

        // ADD THESE
        [SerializeField] private WorldVoxelType[] _voxelTypes;
        public NativeArray<JobWorldVoxelType> jobVoxelTypes;

        public Transform priorityPosition;

        private void Start() {
            Setup();
            StartCoroutine(MeshWorld());
        }

        private void OnDisable() {
            StopAllCoroutines();
            DisposeAll();
            jobVoxelTypes.Dispose();
        }

        private void Setup() {
            // 将玩家设置成世界中心
            priorityPosition.position = new Vector3(
                chunkResolution * gridResolution,
                chunkResolution * gridResolution,
                chunkResolution * gridResolution
            ) / 2;

            // 将WorldVoxelType[]变成NativeArray<JobWorldVoxelType>
            // 为什么不用NativeArray<WorldVoxelType>?
            // 因为WorldVoxelType里面有个string name
            jobVoxelTypes = new NativeArray<JobWorldVoxelType>(_voxelTypes.Length, Allocator.Persistent);
            for (int i = 0; i < _voxelTypes.Length; i++) {
                // create new JobVoxelType
                var jobVoxelType = new JobWorldVoxelType() {
                    isSolid = _voxelTypes[i].isSolid,
                    atlasOffset = _voxelTypes[i].atlasOffset,
                    topAtlasOffset = _voxelTypes[i].topAtlasOffset,
                    bottomAtlasOffset = _voxelTypes[i].bottomAtlasOffset,
                };
                jobVoxelTypes[i] = jobVoxelType;
            }


            var scale = new Vector3(
                chunkResolution,
                chunkResolution,
                chunkResolution
            );
            chunkBounds = new Bounds(scale / 2, scale);

            _chunks = new WorldChunk[gridResolution, gridResolution];
            for (int x = 0; x < gridResolution; x++) {
                for (int z = 0; z < gridResolution; z++) {
                    // 块的位置在xz平面上，一个块占 chunkResolution * chunkResolution的范围
                    Vector3 chunkPos = new Vector3(x, 0f, z) * chunkResolution;
                    _chunks[x, z] = new WorldChunk(this, chunkPos);
                }
            }
        }

        /// <summary>
        /// 测试速度
        /// </summary>
        private void MeshWorld_1() {
            // 这样调度就是，调度一个等待一个完成再调度下一个Job
            //_stopwatch.Start();
            //for (int x = 0; x < gridResolution; x++) {
            //    for (int z = 0; z < gridResolution; z++) {
            //        _chunks[x, z].Schedule();
            //        _chunks[x, z].Complete();
            //    }
            //}
            //_stopwatch.Stop();
            ////419.9993ms
            //lg.i($"{((float)_stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000}ms");

            // 这个是全部调度，等全部完成
            //_stopwatch.Start();
            //for (int x = 0; x < gridResolution; x++) {
            //    for (int z = 0; z < gridResolution; z++) {
            //        _chunks[x, z].Schedule();
            //    }
            //}
            //for (int x = 0; x < gridResolution; x++) {
            //    for (int z = 0; z < gridResolution; z++) {
            //        _chunks[x, z].Complete();
            //    }
            //}
            //_stopwatch.Stop();
            //// 171.0196ms
            //lg.i($"{((float)_stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000}ms");
        }

        private IEnumerator MeshWorld() {
            for (int x = 0; x < gridResolution; x++) {
                for (int z = 0; z < gridResolution; z++) {
                    _chunks[x, z].Schedule();
                    yield return null;
                }
            }

            // loop until all chunks are completed
            while (true) {
                for (int x = 0; x < gridResolution; x++) {
                    for (int z = 0; z < gridResolution; z++) {
                        if (_chunks[x, z].IsCompleted && _chunks[x, z].IsScheduled) {
                            _chunks[x, z].Complete();
                        }

                        yield return null;
                    }
                }
            }
        }

        private void DisposeAll() {
            foreach (var chunk in _chunks) {
                if (chunk.IsScheduled) {
                    chunk.Dispose();
                }
            }
        }
    }
}