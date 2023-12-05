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

            // ����ÿ����ʵ�壬������Ҫ��ȡ���޸���LocalTransform��Velocity
            foreach (var (ballTransform, velocity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Velocity>>()
             .WithAll<Ball>()
             .WithDisabled<Carry>()) {
                // ����ٶ����ֵΪ0���������ƶ�
                if (velocity.ValueRO.Value.Equals(float2.zero)) {
                    continue;
                }
                float magnitude = math.length(velocity.ValueRO.Value);
                var newPosition = ballTransform.ValueRW.Position + new float3(
                    velocity.ValueRO.Value.x,
                    0,
                    velocity.ValueRO.Value.y) * dt;

                // �ж����Ƿ������ϰ���
                foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>()) {
                    if(math.distancesq(newPosition, obstacleTransform.ValueRO.Position) <= minDistSQ) {
                        newPosition = DeflectBall(ballTransform.ValueRO.Position, obstacleTransform.ValueRO.Position, velocity, magnitude, dt);
                        break;
                    }
                }
                ballTransform.ValueRW.Position = newPosition;

                // �ٶ�˥��
                var newMagnitude = math.max(magnitude - decayFactor, 0);
                velocity.ValueRW.Value = math.normalizesafe(velocity.ValueRO.Value) * newMagnitude;
            }
        }

        private float3 DeflectBall(float3 ballPos, float3 obstaclePos, RefRW<Velocity> velocity, float magnitude, float dt) {
            // ���ϰ���->���ָ����
            var obstacleToBallVector = math.normalize((ballPos - obstaclePos).xz);
            // ȷ���µ��ٶȷ���
            velocity.ValueRW.Value = math.reflect(math.normalize(velocity.ValueRO.Value), obstacleToBallVector) * magnitude;
            return ballPos + new float3(velocity.ValueRO.Value.x, 0, velocity.ValueRO.Value.y) * dt;
        }
    }
}