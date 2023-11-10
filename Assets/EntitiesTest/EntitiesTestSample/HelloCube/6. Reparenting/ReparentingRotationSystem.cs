using EntitiesTest.HelloCube.Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;



namespace EntitiesTest.HelloCube.Reparenting {
    public partial struct ReparentingRotationSystem : ISystem {

        bool _attached;
        float _timer;
        const float _Interval = .7f;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            _timer = _Interval;
            _attached = true;
            state.RequireForUpdate<ReparentingComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {

            // 一直旋转
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transformComp, speedComp) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeedComp>>()) {
                transformComp.ValueRW = transformComp.ValueRO.RotateZ(speedComp.ValueRO.RadiansPerSecond * deltaTime);
            }


            _timer -= SystemAPI.Time.DeltaTime;
            if(_timer > 0) {
                return;
            }
            _timer = _Interval;

            // 每0.7s后
            // 会从加入或者移出子物体
            var rotatorEntity = SystemAPI.GetSingletonEntity<RotationSpeedComp>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            if (_attached) {
                DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(rotatorEntity);
                for (int i = 0; i < children.Length; i++) {
                    ecb.RemoveComponent<Parent>(children[i].Value);
                }
            }
            else {
                foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                    .WithNone<RotationSpeedComp>()
                    .WithEntityAccess()) {
                    ecb.AddComponent(entity, new Parent() { Value = rotatorEntity });
                }
            }

            ecb.Playback(state.EntityManager);
            _attached = !_attached;
        }
    }
}
