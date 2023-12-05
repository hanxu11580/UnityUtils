using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Kickball {

    // �������ϰ�����������
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

            // �߼�API
            // ͨ��source-gen���ɵĴ�����������Ĵ���
            // �������һ��С������ObstacleAuthoringû�й���Ԥ���壬����foreach�������û��ִ��
            foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>()) {
                var player = state.EntityManager.Instantiate(config.PlayerPrefab);
                state.EntityManager.SetComponentData(player, new LocalTransform {
                    Position = new float3 {
                        x = obstacleTransform.ValueRO.Position.x + config.PlayerOffset,
                        y = 1,
                        z = obstacleTransform.ValueRO.Position.z + config.PlayerOffset
                    },
                    // ���������Ĭ��Ϊ0
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
#else
            // �ͼ�API
            // SystemAPI.QueryBuilder()�Ỻ������
            var query = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();
            // �ӿ�����������������Ҫ���;��
            var localTransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(true);
            // ִ�в�ѯ����������ʵ�����ѯƥ��Ŀ�
            var chunks = query.ToArchetypeChunkArray(Allocator.Temp);
            foreach (var chunk in chunks) {
                // ʹ��LocalTransform���;���ӿ��л�ȡLocalTransform�����������
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