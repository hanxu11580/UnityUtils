using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.HelloCube.PrefabTest {
    public class SpawnerPrefabAuthoring : MonoBehaviour {

        public GameObject prefab;


        class Baker : Baker<SpawnerPrefabAuthoring> {
            public override void Bake(SpawnerPrefabAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpawnerComp() {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                });
            }
        }

    }

    public struct SpawnerComp : IComponentData {
        public Entity prefab;
    }
}