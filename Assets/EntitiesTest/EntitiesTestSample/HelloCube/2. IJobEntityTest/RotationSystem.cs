using EntitiesTest.HelloCube.Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace EntitiesTest.HelloCube.IJobEntityTest {
    public partial struct RotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<IJobEntityComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new RotateAndScaleJob() {

            };
            job.Schedule();
        }
    }

    // IJobEntity实现了查询过程，source generation of entity帮助我们处理
    // 所有onUpdate不用查询
    [BurstCompile]
    partial struct RotateAndScaleJob : IJobEntity {
        public float detalTime;
        public float elapsedTime;

        // 查询所有具有LocalTransform\PostTransformMatrix\RotationSpeed的实体
        private void Execute(ref LocalTransform localTransform, ref PostTransformMatrix postTransformMatrix, in RotationSpeed speed) {
            localTransform = localTransform.RotateZ(speed.RadiansPerSecond * detalTime);
            postTransformMatrix.Value = float4x4.Scale(1, 1, math.sin(elapsedTime));
        }
    }
}