using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


// https://sparker3d.com/paper/minecraft-unity/16

namespace MinecraftVoxelTerrain {
    //[ExecuteInEditMode]
    public class TerrainGeneratorChunk : MonoBehaviour {

        [SerializeField] TerrainGenerator _terrainGenerator;

        private void Start() {
            // ��ʼ��TerrainGenerator
            _terrainGenerator.Init();

            // ��ChunkResolution�����Χ����������
            for (int x = 0; x < _terrainGenerator.ChunkResolution; x++) {
                for (int y = 0; y < _terrainGenerator.ChunkResolution; y++) {
                    for (int z = 0; z < _terrainGenerator.ChunkResolution; z++) {
                        // ��������߶ȿ��ƣ������Ƿ񻭳������ﵽ����Ч��
                        if (IsSolid(x, y, z)) {
                            MeshVoxel(x, y, z);
                        }
                    }
                }
            }

            // Complete TerrainGenerator
            _terrainGenerator.Complete();
        }

        private void MeshVoxel(int x, int y, int z) {
            Vector3 offsetPos = new Vector3(x, y, z);
            var vexelType = _terrainGenerator.GetVoxelType(x, y, z);
            for (int side = 0; side < 6; side++) {
                // �����ʵ�ĵ�����
                if (IsNeighborSolid(x, y, z, side, vexelType)) {
                    continue;
                }
                _terrainGenerator.Layers[vexelType.layer].MeshQuad(offsetPos, vexelType, side);
            }
        }

        /// <summary>
        /// �Ƿ���ʵ������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private bool IsSolid(int x, int y, int z) {
            // ������Χ�Ĳ���ȥ������
            // ��������˵���ھӳ����Ļ����Ὣ���滭����
            if (x < 0 || x >= _terrainGenerator.ChunkResolution
                || y < 0 || y >= _terrainGenerator.ChunkResolution
                || z < 0 || z >= _terrainGenerator.ChunkResolution) {
                return false;
            }
            return _terrainGenerator.GetVoxelType(x, y, z).isSolid;
        }

        private bool IsNeighborSolid(int x, int y, int z, int side, LayerVoxelType selfVoxelType) {
            int3 neighborOffset = Tables.NeighborOffsets[side];
            var neighborPos = new Vector3Int(x + neighborOffset.x, y + neighborOffset.y, z + neighborOffset.z);
            var neighborVoxelType = _terrainGenerator.GetVoxelType(neighborPos.x, neighborPos.y, neighborPos.z);
            if(neighborVoxelType.layer != selfVoxelType.layer) {
                // ��������֮ǰ�㲻һ�������ử����
                // Ŀǰֻ��ˮ�����ͨ��2�㣬����������ɳ�ӡ���֮��ı߽����ɲ��ử����
                // ֻ��������ɳ�ӡ��ݺ�ˮ�ı߽�ử���߽���
                return false;
            }

            return IsSolid(neighborPos.x, neighborPos.y, neighborPos.z);
        }
    }
}