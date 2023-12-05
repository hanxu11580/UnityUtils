using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EntitiesTest.Kickball {
    // ���UpdateBefore�Ǳ�Ҫ�ģ���ȷ����������֡����ȷλ��
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallSpawnerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BallSpawner>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<Config>();
            // �س�
            if (!Input.GetKeyDown(KeyCode.Return)) {
                return;
            }
            var rand = new Random(123);
            // ÿ��Player����һ���򣬲��Ҹ���һ����ʼ�ٶ�
            foreach (var transform in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Player>()) {
                var ball = state.EntityManager.Instantiate(config.BallPrefab);
                state.EntityManager.SetComponentData(ball, new LocalTransform {
                    Position = transform.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                state.EntityManager.SetComponentData(ball, new Velocity {
                    Value = rand.NextFloat2Direction() * config.BallStartVelocity
                });
            }
        }
    }
}