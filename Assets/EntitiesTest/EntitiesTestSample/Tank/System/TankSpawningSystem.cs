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
            // 得到一个QueryMask用来快速判断某个entity是否符合query
            var queryMask = query.GetEntityQueryMask();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tanks = new NativeArray<Entity>(config.TankCount, Allocator.Temp);
            ecb.Instantiate(config.TankPrefab, tanks);

            // 记录一个命令，该命令根据实体查询掩码为实体的链接实体组设置组件
            // 链接实体组中掩码不匹配的实体将被安全跳过
            foreach (var tank in tanks) {
                ecb.SetComponentForLinkedEntityGroup(tank, queryMask, new URPMaterialPropertyBaseColor { Value = RandomColor(ref random) });
            }
            // ecb.Instantiate(config.TankPrefab, tanks)会生成Temp实体ID
            // 此时会生成真实的实体ID，通过对Temp实体ID的Mapping存如创建好的列表
            ecb.Playback(state.EntityManager);
        }

        private static float4 RandomColor(ref Random random) {
            var hue = (random.NextFloat() + 0.618034005f) % 1;
            return (Vector4)Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}