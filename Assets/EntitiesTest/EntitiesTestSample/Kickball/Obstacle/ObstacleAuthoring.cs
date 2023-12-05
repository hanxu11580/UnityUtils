using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.Kickball {


    public class ObstacleAuthoring : MonoBehaviour {
        class Baker : Baker<ObstacleAuthoring> {
            // 每次重新烘焙创作组件都会调用Bake()
            public override void Bake(ObstacleAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Obstacle>(entity);
            }
        }
    }

    public struct Obstacle : IComponentData {
        /*
         * 空struct被称为tag component，但它们可以像任何其他组件一样被查询
         */
    }
}