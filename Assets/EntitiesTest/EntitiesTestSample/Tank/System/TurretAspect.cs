using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public readonly partial struct TurretAspect : IAspect {
        readonly RefRO<Turret> _Turret;
        readonly RefRO<URPMaterialPropertyBaseColor> _BaseColor;

        public Entity CannonBallSpawn => _Turret.ValueRO.CannonBallSpawn;
        public Entity CannonBallPrefab => _Turret.ValueRO.CannonBallPrefab;
        public float4 Color => _BaseColor.ValueRO.Value;
    }
}