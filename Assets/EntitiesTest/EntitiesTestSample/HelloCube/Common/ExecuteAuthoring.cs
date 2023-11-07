using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace EntitiesTest.HelloCube.Common {
public class ExecuteAuthoring : MonoBehaviour {
    public bool MainThread;
    public bool IJobEntity;
    public bool Aspects;
    public bool Prefabs;
    public bool IJobChunk;
    public bool Reparenting;
    public bool EnableableComponents;
    public bool GameObjectSync;
    class ExecuteAuthoringBaker : Baker<ExecuteAuthoring> {
        public override void Bake(ExecuteAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            if (authoring.MainThread) AddComponent<MainThreadComp>(entity);
            if (authoring.IJobEntity) AddComponent<IJobEntityComp>(entity);
            if (authoring.Aspects) AddComponent<AspectsComp>(entity);
            if (authoring.Prefabs) AddComponent<PrefabsComp>(entity);
            if (authoring.IJobChunk) AddComponent<IJobChunkComp>(entity);
            if (authoring.GameObjectSync) AddComponent<GameObjectSyncComp>(entity);
            if (authoring.Reparenting) AddComponent<ReparentingComp>(entity);
            if (authoring.EnableableComponents) AddComponent<EnableableComponentsComp>(entity);
        }
    }
}
public struct MainThreadComp : IComponentData {
}
public struct IJobEntityComp : IComponentData {
}
public struct AspectsComp : IComponentData {
}
public struct PrefabsComp : IComponentData {
}
public struct IJobChunkComp : IComponentData {
}
public struct GameObjectSyncComp : IComponentData {
}
public struct ReparentingComp : IComponentData {
}
public struct EnableableComponentsComp : IComponentData {
}
}
