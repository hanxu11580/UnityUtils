using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesTest.Kickball {
    public class BallAuthoring : MonoBehaviour {

        class Baker : Baker<BallAuthoring> {
            public override void Bake(BallAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Ball>(entity);
                AddComponent<Velocity>(entity);
                AddComponent<Carry>(entity);
                SetComponentEnabled<Carry>(entity, false);
            }
        }
    }

    public struct Ball : IComponentData { }

    public struct Velocity : IComponentData {
        public float2 Value;
    }
}