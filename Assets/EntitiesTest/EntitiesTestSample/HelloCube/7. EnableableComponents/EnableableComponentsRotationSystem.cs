using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace EntitiesTest.HelloCube.EnableableComponents {
    /// <summary>
    /// תһ��ͣһ��
    /// </summary>
    public partial struct EnableableComponentsRotationSystem : ISystem {

        float _timer;
        const float _interval = 1.3f;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            _timer = _interval;
            state.RequireForUpdate<EnableableComponentsComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;
            _timer -= deltaTime;

            if(_timer < 0) {
                // ��������EnableableRotationSpeedComp���������enable
                // ��enable״̬ȡ��
                foreach (var enableRefRW in SystemAPI.Query<EnabledRefRW<EnableableRotationSpeedComp>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)) {
                    enableRefRW.ValueRW = !enableRefRW.ValueRO;
                }
                _timer = _interval;
            }

            // ����ֻ��������enable������
            foreach (var (transform, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnableableRotationSpeedComp>>()) {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }
}