using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


namespace EntitiesTest.HelloCube.EnableableComponents {
    /// <summary>
    /// 转一会停一会
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
                // 查找所有EnableableRotationSpeedComp组件，忽略enable
                // 将enable状态取反
                foreach (var enableRefRW in SystemAPI.Query<EnabledRefRW<EnableableRotationSpeedComp>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)) {
                    enableRefRW.ValueRW = !enableRefRW.ValueRO;
                }
                _timer = _interval;
            }

            // 这里只会搜索到enable开启的
            foreach (var (transform, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnableableRotationSpeedComp>>()) {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }
}