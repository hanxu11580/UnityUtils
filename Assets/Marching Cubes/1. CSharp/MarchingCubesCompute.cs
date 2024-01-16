using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace MarchingCubes_CSharp {
    /// <summary>
    /// �н������������
    /// ComputeShader�����
    /// </summary>
    public class MarchingCubesCompute {
        public float[] Weights { get; set; }
        float[] _cubeValues;
        public float IsoLevel { get; set; }
        List<Triangle> _triangles;


        public MarchingCubesCompute() {
            _cubeValues = new float[8];
            _triangles = new List<Triangle>();
        }

        public Triangle[] GetTriangles() {
            return _triangles.ToArray();
        }

        public void March() {
            _triangles.Clear();
            for (int x = 0; x < GridMetrics.PointsPerChunk - 1; x++) {
                for (int y = 0; y < GridMetrics.PointsPerChunk - 1; y++) {
                    for (int z = 0; z < GridMetrics.PointsPerChunk - 1; z++) {

                        // �����ComputeShaderʵ�֣���ΪShader��������
                        // ����ֱ������ѭ������-1����
                        //if(x >= GridMetrics.PointsPerChunk - 1
                        //    || y >= GridMetrics.PointsPerChunk - 1
                        //    || y >= GridMetrics.PointsPerChunk - 1) {
                        //    return;
                        //}

                        // ��������嶥������ֵ
                        _cubeValues[0] = Weights[IndexFromCoord(x, y, z + 1)];
                        _cubeValues[1] = Weights[IndexFromCoord(x + 1, y, z + 1)];
                        _cubeValues[2] = Weights[IndexFromCoord(x + 1, y, z)];
                        _cubeValues[3] = Weights[IndexFromCoord(x, y, z)];
                        _cubeValues[4] = Weights[IndexFromCoord(x, y + 1, z + 1)];
                        _cubeValues[5] = Weights[IndexFromCoord(x + 1, y + 1, z + 1)];
                        _cubeValues[6] = Weights[IndexFromCoord(x + 1, y + 1, z)];
                        _cubeValues[7] = Weights[IndexFromCoord(x, y + 1, z)];

                        // ͨ���Ƚ϶�������ֵС��_isoLevel(��ֵ��)
                        // cubeIndex����ʲô
                        // ���趥��0,3С��_isoLevel
                        // ��ʱcubeIndex = 9��0000 1001
                        int cubeIndex = 0;
                        if (_cubeValues[0] < IsoLevel) cubeIndex |= 1;
                        if (_cubeValues[1] < IsoLevel) cubeIndex |= 2;
                        if (_cubeValues[2] < IsoLevel) cubeIndex |= 4;
                        if (_cubeValues[3] < IsoLevel) cubeIndex |= 8;
                        if (_cubeValues[4] < IsoLevel) cubeIndex |= 16;
                        if (_cubeValues[5] < IsoLevel) cubeIndex |= 32;
                        if (_cubeValues[6] < IsoLevel) cubeIndex |= 64;
                        if (_cubeValues[7] < IsoLevel) cubeIndex |= 128;

                        // ����cubeIndexȥ���ұ�triTableȡ�õ�
                        // {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
                        // ����ζ����2�������棬{0, 11, 2}��{8, 11, 0} ע������Ĳ��Ƕ����������Ǳ�����
                        // {0, 11, 2}˵����3����ֱ�����㣬Ȼ�����������
                        int[] edges = MarchingCubesTables.triTable[cubeIndex];

                        Vector3 worldPos = new Vector3(x, y, z);
                        // ÿ��ȡ����������
                        for (int i = 0; edges[i] != -1; i += 3) {
                            // �� i == 0 ʱ,edges[i] = 0
                            // ���Եõ�������Ϊ0�ıߣ������˵�Ķ�������
                            int e00 = MarchingCubesTables.edgeConnections[edges[i]][0];
                            int e01 = MarchingCubesTables.edgeConnections[edges[i]][1];

                            int e10 = MarchingCubesTables.edgeConnections[edges[i + 1]][0];
                            int e11 = MarchingCubesTables.edgeConnections[edges[i + 1]][1];

                            int e20 = MarchingCubesTables.edgeConnections[edges[i + 2]][0];
                            int e21 = MarchingCubesTables.edgeConnections[edges[i + 2]][1];

                            // ���������ߵ���Ϣ���õ��ˣ���Ҫ��ʼ��������������һ����
                            Vector3 v1 = Interp(MarchingCubesTables.cubeCorners[e00], _cubeValues[e00], MarchingCubesTables.cubeCorners[e01], _cubeValues[e01]) + worldPos;
                            Vector3 v2 = Interp(MarchingCubesTables.cubeCorners[e10], _cubeValues[e10], MarchingCubesTables.cubeCorners[e11], _cubeValues[e11]) + worldPos;
                            Vector3 v3 = Interp(MarchingCubesTables.cubeCorners[e20], _cubeValues[e20], MarchingCubesTables.cubeCorners[e21], _cubeValues[e21]) + worldPos;

                            _triangles.Add(new Triangle {
                                v1 = v1,
                                v2 = v2,
                                v3 = v3
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �����Կ����NoiseCompute���ص�
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private int IndexFromCoord(int x, int y, int z) {
            return x + GridMetrics.PointsPerChunk * (y + GridMetrics.PointsPerChunk * z);
        }

        /// <summary>
        /// ����2���㣨Ҳ����һ���ߣ������������Բ�ֵ���һ������
        /// </summary>
        /// <param name="edgeVertex1">��1</param>
        /// <param name="valueAtVertex1">����ֵ</param>
        /// <param name="edgeVertex2">��2</param>
        /// <param name="valueAtVertex2">����ֵ</param>
        /// <returns></returns>
        private Vector3 Interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2) {
            return (edgeVertex1 + (IsoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1));
        }
    }
}