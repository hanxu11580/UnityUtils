using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace EntitiesTest.Kickball {
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BallMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<Config>();
            var dt = SystemAPI.Time.DeltaTime;
            var decayFactor = config.BallVelocityDecay * dt;
            var minDist = config.ObstacleRadius + 0.5f;
            var minDistSQ = minDist * minDist;

            // 对于每个球实体，我们需要读取和修改其LocalTransform和Velocity
            foreach (var (ballTransform, velocity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Velocity>>()
             .WithAll<Ball>()
             .WithDisabled<Carry>()) {
                // 如果速度组件值为0，将不会移动
                if (velocity.ValueRO.Value.Equals(float2.zero)) {
                    continue;
                }
                float magnitude = math.length(velocity.ValueRO.Value);
                var newPosition = ballTransform.ValueRW.Position + new float3(
                    velocity.ValueRO.Value.x,
                    0,
                    velocity.ValueRO.Value.y) * dt;

                // 判断球是否碰到障碍物
                foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>()) {
                    if(math.distancesq(newPosition, obstacleTransform.ValueRO.Position) <= minDistSQ) {
                        newPosition = DeflectBall(ballTransform.ValueRO.Position, obstacleTransform.ValueRO.Position, velocity, magnitude, dt);
                        break;
                    }
                }
                ballTransform.ValueRW.Position = newPosition;

                // 速度衰减
                var newMagnitude = math.max(magnitude - decayFactor, 0);
                velocity.ValueRW.Value = math.normalizesafe(velocity.ValueRO.Value) * newMagnitude;
            }
        }

        private float3 DeflectBall(float3 ballPos, float3 obstaclePos, RefRW<Velocity> velocity, float magnitude, float dt) {
            // 从障碍物->球的指向方向
            var obstacleToBallVector = math.normalize((ballPos - obstaclePos).xz);
            // 确定新的速度方向
            velocity.ValueRW.Value = math.reflect(math.normalize(velocity.ValueRO.Value), obstacleToBallVector) * magnitude;
            return ballPos + new float3(velocity.ValueRO.Value.x, 0, velocity.ValueRO.Value.y) * dt;
        }
    }
}