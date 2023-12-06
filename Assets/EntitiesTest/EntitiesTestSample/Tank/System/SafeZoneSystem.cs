using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EntitiesTest.Tanks {
    /// <summary>
    /// 在圆圈区域内，不会射击
    /// </summary>
    [UpdateBefore(typeof(TurretShootingSystem))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct SafeZoneSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SafeZoneFlag>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float radius = SystemAPI.GetSingleton<Config>().SafeZoneRadius;
            const float debugRenderStepInDegrees = 20;
            for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees) {
                var a = float3.zero;
                var b = float3.zero;
                math.sincos(math.radians(angle), out a.x, out a.z);
                math.sincos(math.radians(angle+ debugRenderStepInDegrees), out b.x, out b.z);
                Debug.DrawLine(a * radius, b * radius);
            }

            var safeZoneJob = new SafeZoneJob {
                SquaredRadius = radius * radius
            };
            safeZoneJob.ScheduleParallel();
        }
    }

    [WithAll(typeof(Turret))]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct SafeZoneJob : IJobEntity {
        public float SquaredRadius;

        // 因为想要子实体全局位置，所以使用LocalToWorld而不是LocalTransform
        private void Execute(in LocalToWorld transformMatrix, EnabledRefRW<Shooting> shootingState) {
            shootingState.ValueRW = math.lengthsq(transformMatrix.Position) > SquaredRadius;
        }
    }
}
