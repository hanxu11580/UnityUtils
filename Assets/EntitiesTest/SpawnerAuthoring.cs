using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using USDT.Utils;


namespace EntitiesTest {
    public class SpawnerAuthoring : MonoBehaviour {
        public GameObject prefab;
        public float spawnRate;
        class SpawnerBaker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                var random = new Unity.Mathematics.Random();
                random.InitState();
                var data = new SpawnerComponentData() {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    spawnPos = authoring.transform.position,
                    nextSpawnTime = 0f,
                    spawnRate = authoring.spawnRate,
                    random = random
                };
                AddComponent(entity, data);
            }
        }
    }
}
