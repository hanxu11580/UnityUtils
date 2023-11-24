using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace EntitiesTest.LineExample.SingleThreadedJob {
    public class FindNearest : MonoBehaviour {
        NativeArray<float3> targetPositions;
        NativeArray<float3> seekerPositions;
        NativeArray<float3> nearestTargetPositions;

        private void Start() {
            Spawner spawner = FindObjectOfType<Spawner>();
            targetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
            seekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
            nearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        }

        private void OnDestroy() {
            targetPositions.Dispose();
            seekerPositions.Dispose();
            nearestTargetPositions.Dispose();
        }

        private void Update() {
            for (int i = 0; i < targetPositions.Length; i++) {
                targetPositions[i] = Spawner.TargetTransforms[i].localPosition;
            }

            for (int i = 0; i < seekerPositions.Length; i++) {
                seekerPositions[i] = Spawner.SeekerTransforms[i].localPosition;
            }

            FindNearestJob findNearestJob = new FindNearestJob() {
                targetPositions = targetPositions,
                seekerPositions = seekerPositions,
                nearestTargetPositions = nearestTargetPositions
            };
            JobHandle jobHandle = findNearestJob.Schedule();
            jobHandle.Complete();

            for (int i = 0; i < seekerPositions.Length; i++) {
                Debug.DrawLine(seekerPositions[i], nearestTargetPositions[i]);
            }
        }
    }
}