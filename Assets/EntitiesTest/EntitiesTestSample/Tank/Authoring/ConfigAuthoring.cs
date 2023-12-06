using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public class ConfigAuthoring : MonoBehaviour {
        public GameObject TankPrefab;
        public int TankCount;
        public float SafeZoneRadius;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config {
                    TankPrefab = GetEntity(authoring.TankPrefab, TransformUsageFlags.Dynamic),
                    TankCount = authoring.TankCount,
                    SafeZoneRadius = authoring.SafeZoneRadius
                });
            }
        }
    }

    public struct Config : IComponentData {
        public Entity TankPrefab;
        public int TankCount;
        public float SafeZoneRadius;
    }
}
