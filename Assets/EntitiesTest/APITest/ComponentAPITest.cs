using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace EntitiesTest {
struct ComponentA : IComponentData { }
struct ComponentB : IComponentData { }
// 将组件添加到单个实体
public partial struct AddComponentToSingletonEntityEg : ISystem {
    public void OnCreate(ref SystemState state) {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<ComponentA>(entity);
    }
}
// 将一个组件添加到多个实体
public partial struct AddComponentToMultipleEntitiesSystemEg : ISystem {
    public void OnCreate(ref SystemState state) {
        // Type会隐式转换为ComponentType
        var query = state.GetEntityQuery(typeof(ComponentA));
        state.EntityManager.AddComponent<ComponentB>(query);
    }
}
// 删除组件
public partial struct RemoveComponentSystemEg : ISystem {
    public void OnCreate(ref SystemState state) {
        var query = state.GetEntityQuery(typeof(ComponentA));
        state.EntityManager.RemoveComponent<ComponentA>(query);
    }
}
}
