using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// �ڵ����
public struct CannonBall : IComponentData {
    public float3 speed;
}


readonly partial struct CannonBallAspect : IAspect {
    public readonly Entity self;
    readonly RefRW<LocalTransform> transform;
    readonly RefRW<CannonBall> cannonBall;

    // ���������Բ��Ǳ���ģ�ֻ�Ƿ���ȡ��
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

        // ��ϵͳ�д���Aspect
        // ��ϵͳ�ⲿ���� ʹ�� EntityManager.GetAspect
        var enetiy = state.EntityManager.CreateEntity();
        CannonBallAspect cannonBall = SystemAPI.GetAspect<CannonBallAspect>(enetiy);
    }
}
