﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


namespace EntitiesTest.HelloCube.Common {
    public class RotationSpeedAuthoring : MonoBehaviour {
        public float DegreesPerSecond = 360.0f;
        class Baker : Baker<RotationSpeedAuthoring> {
            public override void Bake(RotationSpeedAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                AddComponent(entity, new RotationSpeedComp {
                    RadiansPerSecond = math.radians(authoring.DegreesPerSecond)
                });
            }
        }
    }

    public struct RotationSpeedComp : IComponentData {
        public float RadiansPerSecond;
    }
}
