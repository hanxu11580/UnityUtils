using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes {
    /// <summary>
    /// 负责将GPU ComputeShader生成的噪音值返回给C#
    /// </summary>
    public class NoiseGenerator : MonoBehaviour {
        public ComputeShader NoiseShader;

        [SerializeField] float noiseScale = 1f;
        [SerializeField] float amplitude = 5f;
        [SerializeField] float frequency = 0.005f;
        [SerializeField] int octaves = 8;
        [SerializeField, Range(0f, 1f)] float groundPercent = 0.2f;


        private ComputeBuffer _weightsBuffer;
        private int _bufferCount;

        private void Awake() {
            _bufferCount = GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk;
            CreateBuffers();
        }
        private void OnDestroy() {
            ReleaseBuffers();
        }

        private void CreateBuffers() {
            // sizeof(float) float大小
            _weightsBuffer = new ComputeBuffer(_bufferCount, sizeof(float));
        }

        private void ReleaseBuffers() {
            _weightsBuffer.Release();
        }

        public float[] GetNoise() {
            float[] noiseValues = new float[_bufferCount];
            NoiseShader.SetBuffer(0, "_Weights", _weightsBuffer);
            NoiseShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk);
            NoiseShader.SetFloat("_NoiseScale", noiseScale);
            NoiseShader.SetFloat("_Amplitude", amplitude);
            NoiseShader.SetFloat("_Frequency", frequency);
            NoiseShader.SetInt("_Octaves", octaves);
            NoiseShader.SetFloat("_GroundPercent", groundPercent);
            NoiseShader.Dispatch(0, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads);
            _weightsBuffer.GetData(noiseValues);
            return noiseValues;
        }
    }
}