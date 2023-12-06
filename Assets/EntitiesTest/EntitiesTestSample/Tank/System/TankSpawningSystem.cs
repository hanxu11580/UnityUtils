using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EntitiesTest.Tanks {
    public partial struct TankSpawningSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TankSpawningFlag>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<Config>();
            var random = new Random(123);
            var query = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
            // �õ�һ��QueryMask���������ж�ĳ��entity�Ƿ����query
            var queryMask = query.GetEntityQueryMask();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tanks = new NativeArray<Entity>(config.TankCount, Allocator.Temp);
            ecb.Instantiate(config.TankPrefab, tanks);

            // ��¼һ��������������ʵ���ѯ����Ϊʵ�������ʵ�����������
            // ����ʵ���������벻ƥ���ʵ�彫����ȫ����
            foreach (var tank in tanks) {
                ecb.SetComponentForLinkedEntityGroup(tank, queryMask, new URPMaterialPropertyBaseColor { Value = RandomColor(ref random) });
            }
            // ecb.Instantiate(config.TankPrefab, tanks)������Tempʵ��ID
            // ��ʱ��������ʵ��ʵ��ID��ͨ����Tempʵ��ID��Mapping���紴���õ��б�
            ecb.Playback(state.EntityManager);
        }

        private static float4 RandomColor(ref Random random) {
            var hue = (random.NextFloat() + 0.618034005f) % 1;
            return (Vector4)Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}