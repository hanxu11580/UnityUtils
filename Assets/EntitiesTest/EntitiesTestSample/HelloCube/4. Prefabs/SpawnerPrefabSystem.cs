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
            // ����һ��query����������Entities����RotaionSpeedCompҲ����Cubes
            // query�ᱻ���嵽source generation ���Բ���ÿ��update�����´���
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeedComp>().Build();
            // ֻ�������е�cube���������ˣ��Żᴴ��
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