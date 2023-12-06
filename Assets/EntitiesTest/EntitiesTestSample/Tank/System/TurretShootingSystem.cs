using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Tanks {

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TurretShootingSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TurretShootingFlag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (turret, localToWorld) in
         SystemAPI.Query<TurretAspect, RefRO<LocalToWorld>>()
             .WithAll<Shooting>()) {
                // 生成炮弹，并添加组件LocalTransform、CannonBall、URPMaterialPropertyBaseColor
                Entity instance = state.EntityManager.Instantiate(turret.CannonBallPrefab);
                state.EntityManager.SetComponentData(instance, new LocalTransform {
                    Position = SystemAPI.GetComponent<LocalToWorld>(turret.CannonBallSpawn).Position,
                    Rotation = quaternion.identity,
                    Scale = SystemAPI.GetComponent<LocalTransform>(turret.CannonBallPrefab).Scale
                });
                state.EntityManager.SetComponentData(instance, new CannonBall {
                    Velocity = localToWorld.ValueRO.Up * 20.0f
                });
                state.EntityManager.SetComponentData(instance, new URPMaterialPropertyBaseColor {
                    Value = turret.Color
                });
            }
        }
    }
}