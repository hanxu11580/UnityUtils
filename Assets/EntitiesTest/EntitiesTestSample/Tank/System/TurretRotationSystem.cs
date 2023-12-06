using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Tanks {
    /// <summary>
    /// ÅÚËþ×ÔÐý
    /// </summary>
    public partial struct TurretRotationSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TurretRotationFlag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var spin = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);
            foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Turret>()) {
                transform.ValueRW.Rotation = math.mul(spin, transform.ValueRO.Rotation);
            }
        }
    }
}