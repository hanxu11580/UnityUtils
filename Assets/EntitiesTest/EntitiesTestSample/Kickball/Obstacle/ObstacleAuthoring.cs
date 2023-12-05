using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.Kickball {


    public class ObstacleAuthoring : MonoBehaviour {
        class Baker : Baker<ObstacleAuthoring> {
            // ÿ�����º決��������������Bake()
            public override void Bake(ObstacleAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Obstacle>(entity);
            }
        }
    }

    public struct Obstacle : IComponentData {
        /*
         * ��struct����Ϊtag component�������ǿ������κ��������һ������ѯ
         */
    }
}