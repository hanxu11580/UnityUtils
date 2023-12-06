using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.Tanks {
    class TurretAuthoring : MonoBehaviour {
        public GameObject CannonBallPrefab;
        public Transform CannonBallSpawn;

        class Baker : Baker<TurretAuthoring> {
            public override void Bake(TurretAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Turret {
                    CannonBallPrefab = GetEntity(authoring.CannonBallPrefab, TransformUsageFlags.Dynamic),
                    CannonBallSpawn = GetEntity(authoring.CannonBallSpawn, TransformUsageFlags.Dynamic)
                });

                AddComponent<Shooting>(entity);
            }
        }
    }

    public struct Turret : IComponentData {
        public Entity CannonBallPrefab;
        public Entity CannonBallSpawn;
    }

    public struct Shooting : IComponentData, IEnableableComponent { }
}
