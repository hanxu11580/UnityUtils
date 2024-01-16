using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USDT.Utils;

namespace MarchingCubes_ComputeShaderTest {
    public class CubeGrid : MonoBehaviour {
        public Transform cubePrefab;
        public ComputeShader cubeShader;
        public int cubePerAxis = 80;
        public int repetitions = 1000;
        [Range(0, 40)]
        public int randomStrength;
        public bool useGPU;

        private ComputeBuffer _cubePositionBuffer;
        private Transform[] _cubes;
        private float[] _cubesPositions;
        private WaitForSeconds _waitForSeconds;

        private void Awake() {
            _cubePositionBuffer = new ComputeBuffer(cubePerAxis * cubePerAxis, sizeof(float));
            _waitForSeconds = new WaitForSeconds(0.25f);
        }

        private void Start() {
            CreateGrid();
        }

        private void OnDestroy() {
            _cubePositionBuffer.Release();
        }

        private void CreateGrid() {
            _cubes = new Transform[cubePerAxis * cubePerAxis];
            _cubesPositions = new float[cubePerAxis * cubePerAxis];

            for (int x = 0, i = 0; x < cubePerAxis; x++) {
                for (int z = 0; z < cubePerAxis; z++, i++) {
                    _cubes[i] = Instantiate(cubePrefab, transform);
                    _cubes[i].transform.position = new Vector3(x, 0, z);
                }
            }

            StartCoroutine(UpdateCubeGrid());
        }

        private IEnumerator UpdateCubeGrid() {
            while (true) {
                if (useGPU) {
                    UpdatePositionGPU();
                }
                else {
                    UpdatePositionCPU();
                }

                for (int i = 0; i < _cubes.Length; i++) {
                    _cubes[i].localPosition = new Vector3(
                        _cubes[i].localPosition.x,
                        _cubesPositions[i],
                        _cubes[i].localPosition.z);
                }

                yield return _waitForSeconds;
            }
        }

        private void UpdatePositionCPU() {
            for (int i = 0; i < _cubes.Length; i++) {
                for (int j = 0; j < repetitions; j++) {
                    _cubesPositions[i] = Random.Range(-1f, 1) * randomStrength;
                }
            }
        }

        private void UpdatePositionGPU() {
            var kernel = cubeShader.FindKernel("CubesCompute");

            cubeShader.SetBuffer(kernel, "_Positions", _cubePositionBuffer);

            cubeShader.SetInt("_CubesPerAxis", cubePerAxis);
            cubeShader.SetInt("_Repetitions", repetitions);
            cubeShader.SetFloat("_Time", Time.deltaTime);
            cubeShader.SetInt("_RandomStrength", randomStrength);

            // 一个立方体一个工作组
            int workGroups = Mathf.CeilToInt(cubePerAxis / 8.0f);
            cubeShader.Dispatch(kernel, workGroups, workGroups, 1);

            _cubePositionBuffer.GetData(_cubesPositions);
        }
    }
}
