using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesTest.CubeSpawner {
    public class CubeSpawnerAuthoring : MonoBehaviour {
        public GameObject prefab;

        class Baker : Baker<CubeSpawnerAuthoring> {
            public override void Bake(CubeSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CubeSpawner {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    spawnCount = 0,
                    spawnPosition = float3.zero
                });
            }
        }
    }

    public struct CubeSpawner : IComponentData {
        public Entity prefab;
        public int spawnCount;
        public float3 spawnPosition;
    }
}