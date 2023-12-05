using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Kickball {
    public partial struct PlayerMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var config = SystemAPI.GetSingleton<Config>();

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var input = new float3(horizontal, 0, vertical) * SystemAPI.Time.DeltaTime * config.PlayerSpeed;
            if (input.Equals(float3.zero)) {
                return;
            }

            var minDist = config.ObstacleRadius + 0.5f;
            var minDistSQ = minDist * minDist;
            foreach (var playerTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Player>()) {
                var newPos = playerTransform.ValueRO.Position + input;
                foreach (var obstacleTransform in
                         SystemAPI.Query<RefRO<LocalTransform>>()
                             .WithAll<Obstacle>()) {

                    // 如果新位置与玩家的墙相交，不要移动玩家
                    if (math.distancesq(newPos, obstacleTransform.ValueRO.Position) <= minDistSQ) {
                        newPos = playerTransform.ValueRO.Position;
                        break;
                    }
                }


                playerTransform.ValueRW.Position = newPos;
            }
        }
    }
}