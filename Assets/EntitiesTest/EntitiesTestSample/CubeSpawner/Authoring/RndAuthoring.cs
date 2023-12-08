using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace EntitiesTest.CubeSpawner {
    public class RndAuthoring : MonoBehaviour {
        public uint seed = 0;

        class Baker : Baker<RndAuthoring> {
            public override void Bake(RndAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                if(authoring.seed == 0) {
                    authoring.seed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                }
                AddComponent(entity, new RndComponent {
                    seed = authoring.seed,
                    random = new Random(authoring.seed),
                    randomVector = new float3(1, 0, 0)
                });
            }
        }
    }

    public struct RndComponent : IComponentData {
        public uint seed;
        public Random random;
        public float3 randomVector;
    }
}
