using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace EntitiesTest.Kickball {

    /*
     * TransformSystemGroup中的系统从实体的LocalTransform组件计算渲染矩阵
     * UpdateBefore使此系统在TransformSystemGroup之前更新
     * 因此我们生成障碍物将在同一帧而不是下一帧中计算其渲染矩阵
     */
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct ObstacleSpawnerSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<ObstacleSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            // 只想制造一次障碍，禁用系统将停止后续更新
            state.Enabled = false;
            // 如果0个或多个实体具有Config组件，则会抛出异常
            var config = SystemAPI.GetSingleton<Config>();
            var rand = new Random(123);
            var scale = config.ObstacleRadius * 2;
            for (int column = 0; column < config.NumColumns; column++) {
                for (int row = 0; row < config.NumRows; row++) {
                    var obstacle = state.EntityManager.Instantiate(config.ObstaclePrefab);
                    state.EntityManager.SetComponentData(obstacle, new LocalTransform {
                        Position = new float3 {
                            x = (column * config.ObstacleGridCellSize) + rand.NextFloat(config.ObstacleOffset),
                            y = 0,
                            z = (row * config.ObstacleGridCellSize) + rand.NextFloat(config.ObstacleOffset)
                        },
                        Scale = scale,
                        Rotation = quaternion.identity
                    });
                }
            }
        }
    }
}