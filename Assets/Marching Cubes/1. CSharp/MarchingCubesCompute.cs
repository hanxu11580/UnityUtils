using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace MarchingCubes_CSharp {
    /// <summary>
    /// 行进立方体计算类
    /// ComputeShader替代类
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

                        // 这个是ComputeShader实现，因为Shader参数问题
                        // 这里直接上面循环总数-1就行
                        //if(x >= GridMetrics.PointsPerChunk - 1
                        //    || y >= GridMetrics.PointsPerChunk - 1
                        //    || y >= GridMetrics.PointsPerChunk - 1) {
                        //    return;
                        //}

                        // 获得立方体顶点噪音值
                        _cubeValues[0] = Weights[IndexFromCoord(x, y, z + 1)];
                        _cubeValues[1] = Weights[IndexFromCoord(x + 1, y, z + 1)];
                        _cubeValues[2] = Weights[IndexFromCoord(x + 1, y, z)];
                        _cubeValues[3] = Weights[IndexFromCoord(x, y, z)];
                        _cubeValues[4] = Weights[IndexFromCoord(x, y + 1, z + 1)];
                        _cubeValues[5] = Weights[IndexFromCoord(x + 1, y + 1, z + 1)];
                        _cubeValues[6] = Weights[IndexFromCoord(x + 1, y + 1, z)];
                        _cubeValues[7] = Weights[IndexFromCoord(x, y + 1, z)];

                        // 通过比较顶点噪音值小于_isoLevel(等值面)
                        // cubeIndex代表什么
                        // 假设顶点0,3小于_isoLevel
                        // 此时cubeIndex = 9，0000 1001
                        int cubeIndex = 0;
                        if (_cubeValues[0] < IsoLevel) cubeIndex |= 1;
                        if (_cubeValues[1] < IsoLevel) cubeIndex |= 2;
                        if (_cubeValues[2] < IsoLevel) cubeIndex |= 4;
                        if (_cubeValues[3] < IsoLevel) cubeIndex |= 8;
                        if (_cubeValues[4] < IsoLevel) cubeIndex |= 16;
                        if (_cubeValues[5] < IsoLevel) cubeIndex |= 32;
                        if (_cubeValues[6] < IsoLevel) cubeIndex |= 64;
                        if (_cubeValues[7] < IsoLevel) cubeIndex |= 128;

                        // 根据cubeIndex去查找表triTable取得到
                        // {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
                        // 这意味着有2个三角面，{0, 11, 2}和{8, 11, 0} 注意这里的不是定点索引，是边索引
                        // {0, 11, 2}说明这3个别分别有落点，然后组成三角面
                        int[] edges = MarchingCubesTables.triTable[cubeIndex];

                        Vector3 worldPos = new Vector3(x, y, z);
                        // 每次取三个边索引
                        for (int i = 0; edges[i] != -1; i += 3) {
                            // 当 i == 0 时,edges[i] = 0
                            // 可以得到边索引为0的边，两个端点的顶点索引
                            int e00 = MarchingCubesTables.edgeConnections[edges[i]][0];
                            int e01 = MarchingCubesTables.edgeConnections[edges[i]][1];

                            int e10 = MarchingCubesTables.edgeConnections[edges[i + 1]][0];
                            int e11 = MarchingCubesTables.edgeConnections[edges[i + 1]][1];

                            int e20 = MarchingCubesTables.edgeConnections[edges[i + 2]][0];
                            int e21 = MarchingCubesTables.edgeConnections[edges[i + 2]][1];

                            // 现在三条边的信息都得到了，就要开始从这三条边上找一个点
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
        /// 这块可以看这个NoiseCompute返回的
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private int IndexFromCoord(int x, int y, int z) {
            return x + GridMetrics.PointsPerChunk * (y + GridMetrics.PointsPerChunk * z);
        }

        /// <summary>
        /// 根据2个点（也就是一个边），从里面线性插值获得一个顶点
        /// </summary>
        /// <param name="edgeVertex1">点1</param>
        /// <param name="valueAtVertex1">噪音值</param>
        /// <param name="edgeVertex2">点2</param>
        /// <param name="valueAtVertex2">噪音值</param>
        /// <returns></returns>
        private Vector3 Interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2) {
            return (edgeVertex1 + (IsoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1));
        }
    }
}