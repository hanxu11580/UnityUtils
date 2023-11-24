using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace EntitiesTest.HelloCube.EnableableComponents {
    public class EnableableRotationSpeedAuthoring : MonoBehaviour {
        public bool startEnabled;
        public float degreesPerSecond = 360.0f;

        class Baker : Baker<EnableableRotationSpeedAuthoring> {
            public override void Bake(EnableableRotationSpeedAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnableableRotationSpeedComp() {
                    RadiansPerSecond = math.radians(authoring.degreesPerSecond)
                });
                SetComponentEnabled<EnableableRotationSpeedComp>(entity, authoring.startEnabled);
            }
        }
    }

    // IEnableableComponent
    struct EnableableRotationSpeedComp : IComponentData, IEnableableComponent{
        public float RadiansPerSecond;
    }
}