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
    /// 
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CameraSystem : ISystem {
        Entity target;
        Random random;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CameraFlag>();
            random = new Random(123);
        }

        public void OnUpdate(ref SystemState state) {
            if(target == Entity.Null || Input.GetKeyDown(KeyCode.Space)) {
                var tankQuery = SystemAPI.QueryBuilder().WithAll<Tank>().Build();
                var tanks = tankQuery.ToEntityArray(Allocator.Temp);
                if(tanks.Length == 0) {
                    return;
                }
                target = tanks[random.NextInt(tanks.Length)];
            }

            var cameraTransform = CameraSingleton.Instance.transform;
            var tankTransform = SystemAPI.GetComponent<LocalToWorld>(target);
            // 摄像机在tank身后高处
            cameraTransform.position = tankTransform.Position;
            cameraTransform.position -= 10.0f * (Vector3)tankTransform.Forward;
            cameraTransform.position += new Vector3(0, 5f, 0);
            cameraTransform.LookAt(tankTransform.Position);
        }
    }
}
