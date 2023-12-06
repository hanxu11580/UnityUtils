using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public class TankAuthoring : MonoBehaviour {
        class Baker : Baker<TankAuthoring> {
            public override void Bake(TankAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Tank>(entity);
            }
        }
    }

    public struct Tank : IComponentData {
    }
}