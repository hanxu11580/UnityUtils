using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public class CannonBallAuthoring : MonoBehaviour
    {
        class Baker : Baker<CannonBallAuthoring>
        {
            public override void Bake(CannonBallAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CannonBall>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }

    public struct CannonBall : IComponentData {
        public float3 Velocity;
    }
}
