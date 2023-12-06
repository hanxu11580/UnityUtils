using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public partial struct TankMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TankMovementFlag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Tank>().WithEntityAccess()) {
                var pos = transform.ValueRO.Position;
                pos.y = (float)entity.Index;

                var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
                var dir = float3.zero;
                math.sincos(angle, out dir.x, out dir.z);

                transform.ValueRW.Position += dir * dt * 5.0f;
                transform.ValueRW.Rotation = quaternion.RotateY(angle);
            }
        }
    }
}