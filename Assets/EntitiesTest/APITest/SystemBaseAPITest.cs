using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;





public struct Position : IComponentData {
    public float3 value;
}

public struct Velocity : IComponentData {
    public float3 value;
}


[RequireMatchingQueriesForUpdate]
public partial class MySystemBase : SystemBase {
    protected override void OnUpdate() {
        float dt = SystemAPI.Time.DeltaTime;
        Entities
            .WithName("Update_Displacement")
            .ForEach((ref Position position, in Velocity velocity) => {
                position = new Position() {
                    value = position.value + velocity.value * dt
                };
            })
            .Schedule();
    }
}
