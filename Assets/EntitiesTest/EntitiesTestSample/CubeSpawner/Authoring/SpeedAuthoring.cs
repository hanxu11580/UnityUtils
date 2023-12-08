using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EntitiesTest.CubeSpawner {
    public class SpeedAuthoring : MonoBehaviour {

        public float value;

        class Baker : Baker<SpeedAuthoring> {
            public override void Bake(SpeedAuthoring authoring) {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new SpeedComponent {
                    value = authoring.value
                });
            }
        }
    }

    public struct SpeedComponent : IComponentData {
        public float value;
    }
}