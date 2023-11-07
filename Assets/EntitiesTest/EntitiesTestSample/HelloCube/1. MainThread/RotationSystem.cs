using EntitiesTest.HelloCube.Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


namespace EntitiesTest.HelloCube.MainThread {
    public partial struct RotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            // 只有存在MainThread组件才能运行该系统
            state.RequireForUpdate<MainThreadComp>();
        }
        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (localTransform, rotationSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<RotationSpeedComp>>()) {
                localTransform.ValueRW = localTransform.ValueRO.RotateZ(rotationSpeed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }
}
