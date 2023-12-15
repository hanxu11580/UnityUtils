using EntitiesTest.HelloCube.Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace EntitiesTest.HelloCube.IJobEntityTest {
    public partial struct IJobEntityTestRotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<IJobEntityComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new RotateAndScaleJob() {
                detalTime = SystemAPI.Time.DeltaTime,
                elapsedTime = (float)SystemAPI.Time.ElapsedTime
            };
            job.Schedule();
        }
    }

    // IJobEntityʵ���˲�ѯ���̣�source generation of entity�������Ǵ���
    // ����onUpdate���ò�ѯ
    [BurstCompile]
    partial struct RotateAndScaleJob : IJobEntity {
        public float detalTime;
        public float elapsedTime;

        // ��ѯ���о���LocalTransform\PostTransformMatrix\RotationSpeed��ʵ��
        private void Execute(ref LocalTransform localTransform, ref PostTransformMatrix postTransformMatrix, in RotationSpeedComp speed) {
            localTransform = localTransform.RotateZ(speed.RadiansPerSecond * detalTime);
            postTransformMatrix.Value = float4x4.Scale(1, 1, math.sin(elapsedTime));
        }
    }
}