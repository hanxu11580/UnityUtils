using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using USDT.Utils;

[BurstCompile]
public partial struct SpawnerSystem : ISystem {
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (RefRW<SpawnerComponentData> sdata in SystemAPI.Query<RefRW<SpawnerComponentData>>()) {
            if(sdata.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime) {
                // 创建一个新的实体
                Entity newE = state.EntityManager.Instantiate(sdata.ValueRO.prefab);
                // 添加LocalTransform组件
                state.EntityManager.SetComponentData(newE, LocalTransform.FromPosition(sdata.ValueRO.spawnPos));

                //state.EntityManager.SetComponentData(newE, RenderMesh)

                // 制定下次实例时间
                sdata.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + sdata.ValueRO.spawnRate;
            }
        }
    }
}
