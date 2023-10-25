using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using USDT.Utils;

[BurstCompile]
public partial struct SpawnerSystem : ISystem {
    Unity.Mathematics.Random _random;
    public void OnCreate(ref SystemState state) {
        _random = new Unity.Mathematics.Random();
        _random.InitState();
    }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (RefRW<SpawnerComponentData> sdata in SystemAPI.Query<RefRW<SpawnerComponentData>>()) {
            if(sdata.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime) {
                // 创建一个新的实体
                Entity newE = state.EntityManager.Instantiate(sdata.ValueRO.prefab);

                // 添加LocalTransform组件
                var startV = sdata.ValueRO.spawnPos;
                startV.y += _random.NextFloat(1, 11);
                var rq = quaternion.RotateX(_random.NextInt(0, 360));
                var newPos = math.mul(rq, startV);
                state.EntityManager.SetComponentData(newE, LocalTransform.FromPosition(newPos));

                // 制定下次实例时间
                sdata.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + sdata.ValueRO.spawnRate;
            }
        }
    }
}
