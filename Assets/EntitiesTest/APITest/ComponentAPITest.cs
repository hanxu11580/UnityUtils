using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


struct ComponentA : IComponentData { }
struct ComponentB : IComponentData { }

// �������ӵ�����ʵ��
public partial struct AddComponentToSingletonEntityEg : ISystem {
    public void OnCreate(ref SystemState state) {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<ComponentA>(entity);
    }
}

// ��һ�������ӵ����ʵ��
public partial struct AddComponentToMultipleEntitiesSystemEg : ISystem {
    public void OnCreate(ref SystemState state) {
        // Type����ʽת��ΪComponentType
        var query = state.GetEntityQuery(typeof(ComponentA));
        state.EntityManager.AddComponent<ComponentB>(query);
    }
}


// ɾ�����
public partial struct RemoveComponentSystemEg : ISystem {
    public void OnCreate(ref SystemState state) {
        var query = state.GetEntityQuery(typeof(ComponentA));
        state.EntityManager.RemoveComponent<ComponentA>(query);
    }
}

