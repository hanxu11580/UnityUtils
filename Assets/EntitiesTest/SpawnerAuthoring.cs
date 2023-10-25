using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using USDT.Utils;

public class SpawnerAuthoring : MonoBehaviour {
    public GameObject prefab;
    public float spawnRate;


    class SpawnerBaker : Baker<SpawnerAuthoring> {
        public override void Bake(SpawnerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new SpawnerComponentData() {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                spawnPos = authoring.transform.position,
                nextSpawnTime = 0f,
                spawnRate = authoring.spawnRate
            };
            AddComponent(entity, data);
        }
    }
}
