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

            // �������һ��Chunk���ڳ�����4֡������Ҫʹ�ø��ӳ־õķ�����
            _vertices = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _triangles = new NativeArray<int>(36 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _normals = new NativeArray<Vector3>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
            _uvs = new NativeArray<Vector2>(24 * _world.chunkResolution * _world.chunkResolution * _world.chunkResolution, Allocator.Persistent);
        }

        public void Schedule() {
            // ԭ��_vertices��_triangles��_normals��_uvs���������TempJob

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

            // ��ɺ������٣��ظ�ʹ��
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
    /// Ч������ֱ������һ������
    /// </summary>
    public class DynamicWorld : MonoBehaviour {
        public Material material;
        // һ��Chunk��16������
        public int chunkResolution = 16;
        public int atlasSize = 4;
        // ��������״��������
        public int chunkDistance = 5;
        private Dictionary<Vector3, DynamicWorldChunk> _chunksDic = new Dictionary<Vector3, DynamicWorldChunk>();
        private Stopwatch _stopwatch = new Stopwatch();


        [HideInInspector]
        public Bounds chunkBounds;

        [SerializeField] private DynamicWorldVoxelType[] _voxelTypes;
        public NativeArray<JobDynamicWorldVoxelType> jobVoxelTypes;

        public Transform priorityPosition;

        // ������λ��
        private Vector3 _lastRoundedPosition;

        private List<Vector3> _keysToRemove = new List<Vector3>();
        
        // ����ƶ��˼���
        private int _chunksMoved = 0;
        // ��������������Ȧ
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
                // ���������+z�ƶ���2��ľ��� �����(0,0,0)->(0,0,32)
                // _chunksMoved = 2
                _chunksMoved = Mathf.RoundToInt(Vector3.Distance(_lastRoundedPosition, nowPosition) /chunkResolution);

                _lastRoundedPosition = nowPosition;
                // ����ߵ�û�п��λ��ʱ�������ڽ������ɿ�
                CreateChunk(nowPosition);

                // ͣ�����У����������������ɿ�
                StopAllCoroutines();

                // �����ڷ�Χ�Ŀ�
                Cleanup();

                StartCoroutine(MeshWorld());

            }
        }

        private void Setup() {
            // ��WorldVoxelType[]���NativeArray<JobWorldVoxelType>
            // Ϊʲô����NativeArray<WorldVoxelType>?
            // ��ΪWorldVoxelType�����и�string name
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
        /// �������λ��ȷ����λ��
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

            // ���������+z�ƶ���һ��Chunk, _chunksMoved = 2ʱ
            // ������ֻ��Ҫ�������ɵ�������һȦ�͵����ڶ�Ȧ��Chunk�����ˣ�Ҳ����_currentRadius = chunkDistance
            // ������2�����
            // 1. ��һ���������_currentRadius = chunkDistance + 1��ʱ��Ҳ����6����ʱ��ͼ��ȫ�����������ƶ���2��Chunkλ�ã���ʱӦ���Ǵ�_currentRadius = chunkDistance - 1��ʼ��Ҳ���Ǵ�ʱӦ���Ǵ�_currentRadius = 4�����湫ʽ����ľ���4
            // 2. �ڶ�������������ʱ_currentRadius = 1��Ҳ����˵�����һȦ��������ɣ�_chunksMoved = 2���ƶ���2��Chunk�����������_currentRadius = -1����Ҫ�ӵ�һȦ��ʼ���ɡ����_currentRadius = 3������3ȦChunk, �൱������ƶ����˵ڶ�Ȧλ�õĿ飬+z����ʣһȦ�����Ե�һȦ�����������ɣ���ʽ����Ϊ1����һȦ������������, 
            _currentRadius = _currentRadius - _chunksMoved;
            if(_currentRadius < 0) {
                _currentRadius = 0;
            }

            // ��һֱcurrentRadius = 0ʱ
            // �����ѭ����һֱִ�У����ȴ���һȦȫ��������ɺ󣬲Ż����currentRadius = 1��һȦ

            // �������z�᷽������
            while (_currentRadius <= chunkDistance) {
                bool completed = true;

                // �������x�᷽�������
                // for (int i = -1; i <= 1; i++) ��������3��
                // ��currentRadiusΪ0��ʱ������1��
                // ��currentRadiusΪ1��ʱ������3��
                // ��currentRadiusΪ2��ʱ������5��
                // ��currentRadiusΪ3��ʱ������7��
                // ��currentRadiusΪ4��ʱ������9��
                // ��currentRadiusΪ5��ʱ������11��
                // �����������һ������+z�᷽��Ľ�������״(������������)
                // λ�ã���
                for (int i = -_currentRadius; i <= _currentRadius - 1; i++) {
                    // �����ҵ�λ�õ�ƫ��
                    var key = (new Vector3(i, 0, _currentRadius) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }
                // λ�ã���
                // �����������һ������+x�᷽��Ľ�������״(������������)
                // +z���+x���y = 1/2 * x ��������غ�
                // +z��currentRadius - 1�����һ�������ɣ���+x����
                // +x�����-z���ӵ�λ�ò����ɣ���-z��æ����
                for (int i = -_currentRadius + 1; i <= _currentRadius; i++) {
                    var key = (new Vector3(_currentRadius, 0, i) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // λ�ã���
                // �����������һ������-z�᷽��Ľ�������״(������������)
                for (int i = -_currentRadius + 1; i <= _currentRadius; i++) {
                    var key = (new Vector3(i, 0, -_currentRadius) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // λ�ã���
                // �����������һ������-x�᷽��Ľ�������״(������������)
                for (int i = -_currentRadius; i <= _currentRadius - 1; i++) {
                    var key = (new Vector3(-_currentRadius, 0, i) * chunkResolution) + RoundedPlayerPosition();
                    if (!TryCompleteChunkAt(key)) {
                        completed = false;
                    }
                    yield return null;
                }

                // ֻ�е�һȦ��ɺ�ţ��Ὺʼ��һȦ
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
        /// �����ڷ�Χ�ڿ�
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
            // ���λ���ڿ�����½ǣ�����ԭ�㣩
            // ����chunkResolution = 16ʱ��halfChunkSize����(8,0,8);
            // ���Ƿ�������λ��
            var halfResolution = chunkResolution / 2;
            var halfChunkSize = new Vector3(halfResolution, 0, halfResolution);
            var chunkCenter = RoundedPlayerPosition() + halfChunkSize;
            DrawGenerationRange(chunkCenter);
            DrawCleanupRange(chunkCenter);
        }

        /// <summary>
        /// �����Χ�������صķ�Χ
        /// </summary>
        private void DrawGenerationRange(Vector3 chunkCenter) {
            var sizeWidth = chunkDistance * chunkResolution * 2;
            // ���size�������Χ��������chunk�Ĵ�С
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