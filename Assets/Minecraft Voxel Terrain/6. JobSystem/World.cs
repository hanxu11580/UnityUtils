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

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private GameObject _gameObject;

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
        }

        public void Schedule() {
            _vertices = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);
            _triangles = new NativeArray<int>(36 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.TempJob);
            var job = new MeshJob() {
                vertices = _vertices,
                triangles = _triangles,
                chunkResolution = _world.chunkResolution,
                chunkPosition = _position
            };
            _jobHandle = job.Schedule();
            IsScheduled = true;
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

            IsScheduled = false;
        }
    }

    public class World : MonoBehaviour {
        public Material material;
        public int chunkResolution = 16;
        public int gridResolution = 8;

        private WorldChunk[,] _chunks;
        private bool _allChunksCompleted;
        private Stopwatch _stopwatch = new Stopwatch();
        private void Start() {
            Setup();
            MeshWorld();
        }

        private void Update() {
            if (!_allChunksCompleted) {
                CompleteWorld();
            }
        }

        private void Setup() {
            _chunks = new WorldChunk[gridResolution, gridResolution];
            for (int x = 0; x < gridResolution; x++) {
                for (int z = 0; z < gridResolution; z++) {
                    // ���λ����xzƽ���ϣ�һ����ռ chunkResolution * chunkResolution�ķ�Χ
                    Vector3 chunkPos = new Vector3(x, 0f, z) * chunkResolution;
                    _chunks[x, z] = new WorldChunk(this, chunkPos);
                }
            }
        }

        private void MeshWorld() {
            // �������Ⱦ��ǣ�����һ���ȴ�һ������ٵ�����һ��Job
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

            // �����ȫ�����ȣ���ȫ�����
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

            // ȫ����������
            // ����������������̣߳�����2����������
            _stopwatch.Start();
            for (int x = 0; x < gridResolution; x++) {
                for (int z = 0; z < gridResolution; z++) {
                    _chunks[x, z].Schedule();
                }
            }
        }



        private void CompleteWorld() {
            bool allChunkCompleted = true;
            for (int x = 0; x < gridResolution; x++) {
                for (int z = 0; z < gridResolution; z++) {
                    if (_chunks[x, z].IsScheduled) {
                        if(_chunks[x, z].IsCompleted) {
                            _chunks[x, z].Complete();
                        }
                        else {
                            // ����δ��ɵ�Chunk
                            allChunkCompleted = false;
                        }
                    }
                }
            }

            _allChunksCompleted = allChunkCompleted;
            if (allChunkCompleted) {
                // 214.9673ms
                lg.i($"{((float)_stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000}ms");
            }
        }
    }
}