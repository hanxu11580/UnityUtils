using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace EntitiesTest.HelloCube.PrefabTest {
    public partial struct FallAndDestroySystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PrefabsComp>();
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transformComp, speedComp) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeedComp>>()) {
                transformComp.ValueRW = transformComp.ValueRO.RotateZ(speedComp.ValueRO.RadiansPerSecond * deltaTime);
            }

            var ecbSinletion = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSinletion.CreateCommandBuffer(state.WorldUnmanaged);

            var movement = new float3(0, -SystemAPI.Time.DeltaTime * 5f, 0);
            foreach (var (transform, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<RotationSpeedComp>()
                         .WithEntityAccess()) {
                transform.ValueRW.Position += movement;
                if(transform.ValueRO.Position.y < 0) {
                    // 销毁实体是结构更改
                    // 所以记录一个命令，稍后销毁实体
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}