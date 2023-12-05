using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Kickball {
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct NewPlayerMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NewPlayerMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<Config>();
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacle>().Build();
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var input = new float3(horizontal, 0, vertical) * SystemAPI.Time.DeltaTime * config.PlayerSpeed;
            if (input.Equals(float3.zero)) {
                return;
            }
            var minDist = config.ObstacleRadius + 0.5f;
            var job = new PlayerMovementJob {
                Input = input,
                MinDistSQ = minDist * minDist,
                ObstacleTransforms = obstacleQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator)
            };
            job.ScheduleParallel();
        }
    }

    [WithAll(typeof(Player))]
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity {
        [ReadOnly] public NativeArray<LocalTransform> ObstacleTransforms;
        public float3 Input;
        public float MinDistSQ;

        public void Execute(ref LocalTransform transform, ref Player player) {
            var newPos = transform.Position + Input;
            foreach (var obstacleTransforms in ObstacleTransforms) {
                if(math.distancesq(newPos, obstacleTransforms.Position) <= MinDistSQ) {
                    newPos = transform.Position;
                    break;
                }
            }

            player.Dir = math.normalize(new float2(Input.x, Input.z));
            transform.Position = newPos;
        }
    }
}