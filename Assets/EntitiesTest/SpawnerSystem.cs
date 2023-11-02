using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using USDT.Utils;


namespace EntitiesTest {
[BurstCompile]
public partial struct SpawnerSystem : ISystem {
    public void OnCreate(ref SystemState state) { }
    public void OnDestroy(ref SystemState state) { }
    // 普通使用
    //[BurstCompile]
    //public void OnUpdate(ref SystemState state) {
    //    foreach (RefRW<SpawnerComponentData> sdata in SystemAPI.Query<RefRW<SpawnerComponentData>>()) {
    //        if(sdata.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime) {
    //            // 创建一个新的实体
    //            Entity newE = state.EntityManager.Instantiate(sdata.ValueRO.prefab);
    //            // 添加LocalTransform组件
    //            var startV = sdata.ValueRO.spawnPos;
    //            startV.y += _random.NextFloat(1, 11);
    //            var rq = quaternion.RotateX(_random.NextInt(0, 360));
    //            var newPos = math.mul(rq, startV);
    //            state.EntityManager.SetComponentData(newE, LocalTransform.FromPosition(newPos));
    //            // 制定下次实例时间
    //            sdata.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + sdata.ValueRO.spawnRate;
    //        }
    //    }
    //}
    // Job
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
        var job = new ProcessSpawnerJob() {
            elapsedtime = (float)SystemAPI.Time.ElapsedTime,
            ecb = ecb,
        };
        job.ScheduleParallel();
    }
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecb;
        public float elapsedtime;
        private void Execute([ChunkIndexInQuery] int chunkIndex, ref SpawnerComponentData spawner) {
            if(spawner.nextSpawnTime < elapsedtime) {
                Entity newE = ecb.Instantiate(chunkIndex, spawner.prefab);
                var startV = spawner.spawnPos;
                startV.y += spawner.random.NextFloat(1, 11);
                var rq = quaternion.RotateX(spawner.random.NextInt(0, 360));
                var newPos = math.mul(rq, startV);
                ecb.SetComponent(chunkIndex, newE, LocalTransform.FromPosition(newPos));
                spawner.nextSpawnTime = elapsedtime + spawner.spawnRate;
            }
        }
    }
}
}
