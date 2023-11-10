using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace EntitiesTest.HelloCube.AspectsTest {
    public partial struct AspectsTestRotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AspectsComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            // 不使用Aspects，类似MainThread
            // 旋转
            foreach (var (localtransformComp, speedComp) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeedComp>>()) {
                localtransformComp.ValueRW = localtransformComp.ValueRO.RotateZ(speedComp.ValueRO.RadiansPerSecond * deltaTime);
            }

            // 使用Aspects
            foreach (var movement in SystemAPI.Query<VerticalMovementAspect>()) {
                movement.Move(elapsedTime);
            }

        }
    }

    readonly partial struct VerticalMovementAspect : IAspect {
        readonly RefRW<LocalTransform> _transformComp;
        readonly RefRO<RotationSpeedComp> _speedComp;

        public void Move(double elapsedTime) {
            _transformComp.ValueRW.Position.y = (float)math.sin(elapsedTime * _speedComp.ValueRO.RadiansPerSecond);
        }
    }
}