using Unity.Entities;
using UnityEngine;

namespace EntitiesTest.Tanks
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        public bool TurretRotation;

        public bool TankMovement;

        public bool TurretShooting;

        public bool CannonBall;

        public bool TankSpawning;

        public bool SafeZone;

        public bool Camera;

        class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.TurretRotation) AddComponent<TurretRotationFlag>(entity);
                if (authoring.TankMovement) AddComponent<TankMovementFlag>(entity);
                if (authoring.TurretShooting) AddComponent<TurretShootingFlag>(entity);
                if (authoring.CannonBall) AddComponent<CannonBallFlag>(entity);
                if (authoring.TankSpawning) AddComponent<TankSpawningFlag>(entity);
                if (authoring.SafeZone) AddComponent<SafeZoneFlag>(entity);
                if (authoring.Camera) AddComponent<CameraFlag>(entity);
            }
        }
    }

    public struct TurretRotationFlag : IComponentData
    {
    }

    public struct TankMovementFlag : IComponentData
    {
    }

    public struct TurretShootingFlag : IComponentData
    {
    }

    public struct CannonBallFlag : IComponentData
    {
    }

    public struct CameraFlag : IComponentData
    {
    }

    public struct SafeZoneFlag : IComponentData
    {
    }

    public struct TankSpawningFlag : IComponentData
    {
    }
}
