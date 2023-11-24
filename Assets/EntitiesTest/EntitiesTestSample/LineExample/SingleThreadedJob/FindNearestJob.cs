using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace EntitiesTest.LineExample.SingleThreadedJob {
    [BurstCompile]
    public struct FindNearestJob : IJob {

        [ReadOnly] public NativeArray<float3> targetPositions;
        [ReadOnly] public NativeArray<float3> seekerPositions;

        public NativeArray<float3> nearestTargetPositions;

        public void Execute() {
            // 找到targetPositions离最近的位置,对应存到nearestTargetPositions
            for (int i = 0; i < seekerPositions.Length; i++) {
                float3 seekerPos = seekerPositions[i];
                float nearestDistSq = float.MaxValue;
                for (int j = 0; j < targetPositions.Length; j++) {
                    float3 targetPos = targetPositions[j];
                    float distSq = math.distancesq(seekerPos, targetPos);
                    if(distSq < nearestDistSq) {
                        nearestDistSq = distSq;
                        nearestTargetPositions[i] = targetPos;
                    }
                }
            }    
        }
    }
}
