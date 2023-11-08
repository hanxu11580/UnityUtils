using EntitiesTest.HelloCube.Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesTest.HelloCube.PrefabTest {
    public partial struct SpawnerPrefabSystem : ISystem {

        uint updateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SpawnerComp>();
            state.RequireForUpdate<PrefabsComp>();
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            // 创建一个query来查找所有Entities包含RotaionSpeedComp也就是Cubes
            // query会被缓冲到source generation 所以不会每次update都重新创建
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeedComp>().Build();
            // 只有在所有的cube都被销毁了，才会创建
            if (spinningCubesQuery.IsEmpty) {
                var prefab = SystemAPI.GetSingleton<SpawnerComp>().prefab;
                var instances = state.EntityManager.Instantiate(prefab, 500, Allocator.Temp);
                var random = Random.CreateFromIndex(updateCounter++);
                foreach (var instance in instances) {
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(instance);
                    transform.ValueRW.Position = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 20;
                }
            }
        }
    }
}