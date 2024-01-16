using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes_ComputeShader {
    public class NoiseGenerator : MonoBehaviour {
        ComputeBuffer _weightsBuffer;
        public ComputeShader NoiseShader;

        [SerializeField] float noiseScale = 0.08f;
        [SerializeField] float amplitude = 200;
        [SerializeField] float frequency = 0.004f;
        [SerializeField] int octaves = 6;
        [SerializeField, Range(0f, 1f)] float groundPercent = 0.2f;

        public float[] GetNoise(int lod) {
            CreateBuffers(lod);
            float[] noiseValues =
                new float[GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod)];

            NoiseShader.SetBuffer(0, "_Weights", _weightsBuffer);

            NoiseShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk(lod));
            NoiseShader.SetFloat("_NoiseScale", noiseScale);
            NoiseShader.SetInt("_Scale", GridMetrics.Scale);
            NoiseShader.SetFloat("_Amplitude", amplitude);
            NoiseShader.SetFloat("_Frequency", frequency);
            NoiseShader.SetInt("_Octaves", octaves);
            // 假设我们的 GroundPercent 为 0.5。这意味着我们在 LOD 0 (ChunkSize = 8) 处的地面高度等于 4。当我们使用 LOD 4 (ChunkSize = 40) 时，地面高度等于 20
            NoiseShader.SetFloat("_GroundPercent", groundPercent);

            NoiseShader.SetInt("_GroundLevel", GridMetrics.GroundLevel);


            NoiseShader.Dispatch(
                0, GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod), GridMetrics.ThreadGroups(lod));

            _weightsBuffer.GetData(noiseValues);

            ReleaseBuffers();
            return noiseValues;
        }

        void CreateBuffers(int lod) {
            _weightsBuffer = new ComputeBuffer(
                GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod) * GridMetrics.PointsPerChunk(lod), sizeof(float)
            );
        }

        void ReleaseBuffers() {
            _weightsBuffer.Release();
        }
    }
}