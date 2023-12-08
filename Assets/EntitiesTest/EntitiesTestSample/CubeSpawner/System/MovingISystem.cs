using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

namespace EntitiesTest.CubeSpawner {
    [BurstCompile]
    public partial struct MovingISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new MovingJob() { 
                deltaTime = SystemAPI.Time.DeltaTime,
                elapsedTime = SystemAPI.Time.ElapsedTime
            };

            job.ScheduleParallel();
        }
    }

    readonly partial struct MovingAspect : IAspect {
        private readonly Entity entity;
        private readonly RefRO<SpeedComponent> speedComp;
        private readonly RefRW<RndComponent> rndComp;
        private readonly RefRW<LocalTransform> transformComp;

        public void Move(float deltaTime, double elapsedTime) {
            if (elapsedTime % 3 < deltaTime) {
                rndComp.ValueRW.randomVector = rndComp.ValueRW.random.NextFloat3Direction();
            }
            transformComp.ValueRW.Position += rndComp.ValueRW.randomVector * speedComp.ValueRO.value * deltaTime;
        }
    }

    [BurstCompile]
    partial struct MovingJob : IJobEntity {
        public float deltaTime;
        public double elapsedTime;

        [BurstCompile]
        public void Execute(MovingAspect aspect) {
            aspect.Move(deltaTime, elapsedTime);
        }
    }
}