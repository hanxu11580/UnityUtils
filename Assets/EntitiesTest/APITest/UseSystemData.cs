using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

//定义管理系统数据


public struct UseSystemDataComponentData :IComponentData{
    public float x;
    public float y;
}

public partial struct UseSystemDataSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.EntityManager.AddComponent<UseSystemDataComponentData>(state.SystemHandle);
    }

    public void OnDestroy(ref SystemState state) {

    }
    
    public void OnUpdate(ref SystemState state) {
        SystemAPI.SetComponent(state.SystemHandle, new UseSystemDataComponentData {
            x = 1,
            y = 2
        });
    }
}