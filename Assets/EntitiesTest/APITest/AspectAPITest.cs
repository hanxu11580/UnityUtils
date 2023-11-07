using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace EntitiesTest {
// 炮弹组件
public struct CannonBall : IComponentData {
    public float3 speed;
}
readonly partial struct CannonBallAspect : IAspect {
    public readonly Entity self;
    readonly RefRW<LocalTransform> transform;
    readonly RefRW<CannonBall> cannonBall;
    // 这两个属性不是必须的，只是方便取用
    public float3 Postion
    {
        get => transform.ValueRO.Position;
        set => transform.ValueRW.Position = value;
    }
    public float3 Speed
    {
        get => cannonBall.ValueRO.speed;
        set => cannonBall.ValueRW.speed = value;
    }
}
public partial struct MySystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        foreach (var item in SystemAPI.Query<CannonBallAspect>()) {
        }
        // 在系统中创建Aspect
        // 在系统外部创建 使用 EntityManager.GetAspect
        //var enetiy = state.EntityManager.CreateEntity();
        //CannonBallAspect cannonBall = SystemAPI.GetAspect<CannonBallAspect>(enetiy);
    }
}
}
