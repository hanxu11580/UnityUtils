using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Kickball {

    // 先生成障碍物，再生成玩家
    [UpdateAfter(typeof(ObstacleSpawnerSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerSpawnerSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerSpawner>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<Config>();

#if true

            // 高级API
            // 通过source-gen生成的代码类型下面的代码
            // 这里出现一个小插曲，ObstacleAuthoring没有挂在预制体，导致foreach下面代码没有执行
            foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>()) {
                var player = state.EntityManager.Instantiate(config.PlayerPrefab);
                state.EntityManager.SetComponentData(player, new LocalTransform {
                    Position = new float3 {
                        x = obstacleTransform.ValueRO.Position.x + config.PlayerOffset,
                        y = 1,
                        z = obstacleTransform.ValueRO.Position.z + config.PlayerOffset
                    },
                    // 如果不设置默认为0
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
#else
            // 低级API
            // SystemAPI.QueryBuilder()会缓存数据
            var query = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();
            // 从块访问组件数据数组需要类型句柄
            var localTransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(true);
            // 执行查询：返回所有实体与查询匹配的块
            var chunks = query.ToArchetypeChunkArray(Allocator.Temp);
            foreach (var chunk in chunks) {
                // 使用LocalTransform类型句柄从块中获取LocalTransform组件数据数组
                var localTransforms = chunk.GetNativeArray(ref localTransformTypeHandle);
                for (int i = 0; i < chunk.Count; i++) {
                    var obstacleTransform = localTransforms[i];
                    var player = state.EntityManager.Instantiate(config.PlayerPrefab);
                    state.EntityManager.SetComponentData(player, new LocalTransform {
                        Position = new float3 {
                            x = obstacleTransform.Position.x + config.PlayerOffset,
                            y = 1,
                            z = obstacleTransform.Position.z + config.PlayerOffset
                        },
                        Scale = 1,
                        Rotation = quaternion.identity
                    });
                }
            }
#endif
        }
    }
}