using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesTest.Kickball {
    public class PlayerAuthoring : MonoBehaviour {
        class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Player>(entity);
                AddComponent<Carry>(entity);
                SetComponentEnabled<Carry>(entity, false);
            }
        }
    }

    public struct Player : IComponentData {
        public float2 Dir;
    }

    /// <summary>
    /// ������Target��˫���
    /// ����Carry���Target��Player
    /// Player��Carry���Target����
    /// </summary>
    public struct Carry : IComponentData, IEnableableComponent {
        public Entity Target;
    }
}