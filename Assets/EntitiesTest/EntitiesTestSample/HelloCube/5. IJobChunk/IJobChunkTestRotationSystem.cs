using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.HelloCube.IJobChunkTest {

    public partial struct IJobChunkTestRotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<IJobChunkComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeedComp, LocalTransform>().Build();
            var job = new RotationJobChunk() {
                transformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(),
                rotationSpeedCompTypeHandle = SystemAPI.GetComponentTypeHandle<RotationSpeedComp>(true),
                deltaTime = SystemAPI.Time.DeltaTime
            };

            //与IJobEntity不同，IJobChunk必须手动传递查询。
            //此外，IJobChunk不会隐式地传递和分配state.Dependency JobHandle。
            //（这种传递和分配状态的模式。依赖性确保实体作业调度
            //在不同的系统中将根据需要相互依赖。
            state.Dependency = job.Schedule(spinningCubesQuery, state.Dependency);
        }
    }

    struct RotationJobChunk : IJobChunk {
        public ComponentTypeHandle<LocalTransform> transformTypeHandle;

        [ReadOnly]
        public ComponentTypeHandle<RotationSpeedComp> rotationSpeedCompTypeHandle;

        public float deltaTime;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
            /*
             * 当块中的一个或多个实体具有禁用的查询组件时，useEnableMask参数为true。如果没有查询组件类型实现IEnableComponent，则我们可以假设useEnabledMask将始终为false。但是，添加此保护检查是一个很好的做法，以防稍后有人更改查询或组件类型。
             */
            Assert.IsFalse(useEnabledMask);
            var transforms = chunk.GetNativeArray(ref transformTypeHandle);
            var rotationSpeeds = chunk.GetNativeArray(ref rotationSpeedCompTypeHandle);
            for (int i = 0; i < chunk.Count; i++) {
                transforms[i] = transforms[i].RotateZ(rotationSpeeds[i].RadiansPerSecond * deltaTime);
            }
        }
    }
}