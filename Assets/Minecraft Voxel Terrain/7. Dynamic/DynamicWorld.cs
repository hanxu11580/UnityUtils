using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using USDT.Utils;

// https://sparker3d.com/paper/minecraft-terrain-unity-jobs/42

namespace MinecraftVoxelTerrain {

    public class DynamicWorldChunk {
        public bool IsScheduled { get; private set; }
        public bool IsCompleted => _jobHandle.IsCompleted;

        public GameObject gameObject;

        private JobHandle _jobHandle;
        private NativeArray<Vector3> _vertices;
        private NativeArray<int> _triangles;
        private NativeArray<Vector3> _normals;
        private NativeArray<Vector2> _uvs;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        private Vector3 _position;
        private DynamicWorld _world;

        public DynamicWorldChunk(DynamicWorld world, Vector3 pos) {
            // set references
            this._world = world;
            this._position = pos;

            gameObject = new GameObject();
            gameObject.transform.position = pos;
            gameObject.transform.parent = world.transform;

            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = world.material;
            _meshCollider = gameObject.AddComponent<MeshCollider>();

            // 我们完成一个Chunk现在超过了4帧，所以要使用更加持久的分配器
            _vertices = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _triangles = new NativeArray<int>(36 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _normals = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _uvs = new NativeArray<Vector2>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
        }

        public void Schedule() {
            // 原来_vertices、_triangles、_normals、_uvs在这里分配TempJob

            var job = new DynamicMeshJob() {
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

            // 完成后不在销毁，重复使用
            //_vertices.Dispose();
            //_triangles.Dispose();
            //_normals.Dispose();
            //_uvs.Dispose();

            IsScheduled = false;
        }

        public void Dispose() {
            _jobHandle.Complete();
            _vertices.Dispose();
            _triangles.Dispose();
            _normals.Dispose();
            _uvs.Dispose();
            IsScheduled = false;
            GameObject.Destroy(gameObject);
        }
    }

    /// <summary>
    /// 效果就是直接生成一大块地区
    /// </summary>
    public class DynamicWorld : MonoBehaviour {
        public Material material;
        // 一个Chunk有16个体素
        public int chunkResolution = 16;
        public int atlasSize = 4;
        // 金字塔形状方块的深度
        public int chunkDistance = 5;
        private Dictionary<Vector3, DynamicWorldChunk> _chunksDic = new Dictionary<Vector3, DynamicWorldChunk>();
        private Stopwatch _stopwatch = new Stopwatch();


        [HideInInspector]
        public Bounds chunkBounds;

        [SerializeField] private DynamicWorldVoxelType[] _voxelTypes;
        public NativeArray<JobDynamicWorldVoxelType> jobVoxelTypes;

        public Transform priorityPosition;

        // 玩家最后位置
        private Vector3 _lastRoundedPosition;

        private List<Vector3> _keysToRemove = new List<Vector3>();
        
        // 玩家移动了几块
        private int _chunksMoved = 0;
        // 现在正在生成哪圈
        private int _currentRadius = 0;

        private void Start() {
            Setup();
            StartCoroutine(MeshWorld());
        }

        private void OnDisable() {
            StopAllCoroutines();
            DisposeAll();
            jobVoxelTypes.Dispose();
        }

        private void Update() {
            var nowPosition = RoundedPlayerPosition();
            if (_lastRoundedPosition != nowPosition) {
                // 假如玩家向+z移动了2块的距离 比如从(0,0,0)->(0,0,32)
                // _chunksMoved = 2
                _chunksMoved = Mathf.RoundToInt(Vector3.Distance(_lastRoundedPosition, nowPosition) /chunkResolution);

                _lastRoundedPosition = nowPosition;
                // 玩家走到没有块的位置时，将会在脚下生成块
                CreateChunk(nowPosition);

                // 停掉所有，重新在玩家身边生成块
                StopAllCoroutines();

                // 清理不在范围的块
                Cleanup();

                StartCoroutine(MeshWorld());

            }
        }

        private void Setup() {
            // 将WorldVoxelType[]变成NativeArray<JobWorldVoxelType>
            // 为什么不用NativeArray<WorldVoxelType>?
            // 因为WorldVoxelType里面有个string name
            jobVoxelTypes = new NativeArray<JobDynamicWorldVoxelType>(_voxelTypes.Length, Allocator.Persistent);
            for (int i = 0; i < _voxelTypes.Length; i++) {
                // create new JobVoxelType
                var jobVoxelType = new JobDynamicWorldVoxelType() {
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

            priorityPosition.position = new Vector3(0, chunkResolution / 2, 0);
            var chunkPosition = RoundedPlayerPosition();
            CreateChunk(chunkPosition);
        }

        /// <summary>
        /// 根据玩家位置确定块位置
        /// </summary>
        /// <returns></returns>
        private Vector3 RoundedPlayerPosition() {
            float halfChunkRes = chunkResolution / 2;
            var x = Mathf.Round((priorityPosition.position.x - halfChunkRes) / chunkResolution) * chunkResolution;
            var z = Mathf.Round((priorityPosition.position.z - halfChunkRes) / chunkResolution) * chunkResolution;
            return new Vector3(x, 0, z);
        }

        private void CreateChunk(Vector3 chunkPositon) {
            if (_chunksDic.TryGetValue(chunkPositon, out DynamicWorldChunk chunk)) {
                if (chunk.IsScheduled) {
                    chunk.Complete();
                }
            }
            else {
                chunk = new DynamicWorldChunk(this, chunkPositon);
                chunk.Schedule();
                chunk.Complete();
                _chunksDic.Add(chunkPositon, chunk);
            }
        }

        private IEnumerator MeshWorld() {

            // 假如玩家向+z移动了一个Chunk, _chunksMoved = 2时
            // 理论上只需要重新生成到倒数第一圈和倒数第二圈的Chunk就行了，也就是_currentRadius = chunkDistance
            // 这里有2个情况
            // 1. 第一个情况：当_currentRadius = chunkDistance + 1的时候也就是6，此时地图完全生成完后，玩家移动了2个Chunk位置，此时应该是从_currentRadius = chunkDistance - 1开始跑也就是此时应该是从_currentRadius = 4，下面公式计算的就是4
            // 2. 第二个情况：比如此时_currentRadius = 1，也就是说，块第一圈才生成完成，_chunksMoved = 2，移动了2个Chunk，下面计算了_currentRadius = -1，需要从第一圈开始生成。如果_currentRadius = 3生成了3圈Chunk, 相当于玩家移动到了第二圈位置的块，+z方向还剩一圈，所以第一圈不用重新生成，公式计算为1，第一圈不用重新生成, 
            _currentRadius = _currentRadius - _chunksMoved;
            if(_currentRadius < 0) {
                _currentRadius = 0;
            }

            // 当一直currentRadius = 0时
            // 下面的循环会一直执行，并等待第一圈全部生成完成后，才会进入currentRadius = 1下一圈

            // 这个控制z轴方向数量
            while (_currentRadius <= chunkDistance) {
                bool completed = true;

                // 这个控制x轴方向的数量
                // for (int i = -1; i <= 1; i++) 将会生成3个
                // 当currentRadius为0的时候生成1个
                // 当currentRadius为1的时候生成3个
                // 当currentRadius为2的时候生成5个
                // 当currentRadius为3的时候生成7个
                // 当currentRadius为4的时候生成9个
                // 当currentRadius为5的时候生成11个
                // 这个就生成了一个沿着+z轴方向的金字塔形状(从左往右生成)
                // 位置：上
                for (int i = -_currentRadius; i <= _currentRadius - 1; i++) {
                    // 添加玩家的位置的偏移
                    var key = (new Vector3(i, 0, _currentRadius) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }
                // 位置：右
                // 这个就生成了一个沿着+x轴方向的金字塔形状(从下往上生成)
                // +z轴和+x最后y = 1/2 * x 这个方向重合
                // +z轴currentRadius - 1，最后一个不生成，由+x生成
                // +x下面和-z交接的位置不生成，由-z帮忙生成
                for (int i = -_currentRadius + 1; i <= _currentRadius; i++) {
                    var key = (new Vector3(_currentRadius, 0, i) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // 位置：下
                // 这个就生成了一个沿着-z轴方向的金字塔形状(从左往右生成)
                for (int i = -_currentRadius + 1; i <= _currentRadius; i++) {
                    var key = (new Vector3(i, 0, -_currentRadius) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // 位置：左
                // 这个就生成了一个沿着-x轴方向的金字塔形状(从下往上生成)
                for (int i = -_currentRadius; i <= _currentRadius - 1; i++) {
                    var key = (new Vector3(-_currentRadius, 0, i) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // 只有当一圈完成后才，会开始下一圈
                if (completed) {
                    _currentRadius += 1;
                }
            }
            yield return null;
        }

        private bool TryCompleteChunkAt(Vector3 key) {
            if (_chunksDic.TryGetValue(key, out DynamicWorldChunk chunk)) {

                if (chunk.IsScheduled) {
                    if (!chunk.IsCompleted) {
                        return false;
                    }
                    chunk.Complete();
                }
                return true;
            }

            chunk = new DynamicWorldChunk(this, key);
            _chunksDic.Add(key, chunk);
            chunk.Schedule();
            return false;
        }

        /// <summary>
        /// 清理不在范围内块
        /// </summary>
        private void Cleanup() {
            var radius = chunkDistance * chunkResolution;
            float maxRadius = new Vector3(radius, 0, radius).magnitude;
            _keysToRemove.Clear();
            foreach (var chunkKV in _chunksDic) {
                float distanceFromPlayer = Vector3.Distance(
                    chunkKV.Value.gameObject.transform.position,
                    RoundedPlayerPosition());
                if(distanceFromPlayer > maxRadius) {
                    _keysToRemove.Add(chunkKV.Key);
                }
            }

            foreach (var rmKey in _keysToRemove) {
                var chunk = _chunksDic[rmKey];
                chunk.Dispose();
                _chunksDic.Remove(rmKey);
            }
        }


        private void DisposeAll() {
            foreach (var chunk in _chunksDic.Values) {
                if (chunk.IsScheduled) {
                    chunk.Dispose();
                }
            }
        }

        private void OnDrawGizmos() {
            // 玩家位置在块的左下角（类似原点）
            // 举例chunkResolution = 16时，halfChunkSize就是(8,0,8);
            // 就是方块中心位置
            var halfResolution = chunkResolution / 2;
            var halfChunkSize = new Vector3(halfResolution, 0, halfResolution);
            var chunkCenter = RoundedPlayerPosition() + halfChunkSize;
            DrawGenerationRange(chunkCenter);
            DrawCleanupRange(chunkCenter);
        }

        /// <summary>
        /// 玩家周围生成体素的范围
        /// </summary>
        private void DrawGenerationRange(Vector3 chunkCenter) {
            var sizeWidth = chunkDistance * chunkResolution * 2;
            // 这个size是玩家周围生成所有chunk的大小
            var size = new Vector3(sizeWidth, 0, sizeWidth);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(chunkCenter, size);

            Gizmos.color = Color.blue;
            var currentRadiusWireCubeSize = new Vector3(_currentRadius * chunkResolution * 2, 0, _currentRadius * chunkResolution * 2);
            Gizmos.DrawWireCube(chunkCenter, currentRadiusWireCubeSize);
        }

        private void DrawCleanupRange(Vector3 chunkCenter) {
            Gizmos.color = Color.red;
            var radius = chunkDistance * chunkResolution;
            float maxRadius = new Vector3(radius, 0, radius).magnitude;
            Gizmos.DrawWireSphere(chunkCenter, maxRadius);
        }
    }
}