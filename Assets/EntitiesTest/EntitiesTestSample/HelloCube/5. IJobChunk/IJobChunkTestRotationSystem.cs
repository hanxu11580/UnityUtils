using EntitiesTest.HelloCube.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesTest.HelloCube.IJobChunkTest {

    public partial struct IJobChunkTestRotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<IJobChunkComp>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeedComp, LocalTransform>().Build();
            var job = new RotationJobChunk() {
                transformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(),
                rotationSpeedCompTypeHandle = SystemAPI.GetComponentTypeHandle<RotationSpeedComp>(true),
                deltaTime = SystemAPI.Time.DeltaTime
            };

            //��IJobEntity��ͬ��IJobChunk�����ֶ����ݲ�ѯ��
            //���⣬IJobChunk������ʽ�ش��ݺͷ���state.Dependency JobHandle��
            //�����ִ��ݺͷ���״̬��ģʽ��������ȷ��ʵ����ҵ����
            //�ڲ�ͬ��ϵͳ�н�������Ҫ�໥������
            state.Dependency = job.Schedule(spinningCubesQuery, state.Dependency);
        }
    }

    struct RotationJobChunk : IJobChunk {
        public ComponentTypeHandle<LocalTransform> transformTypeHandle;

        [ReadOnly]
        public ComponentTypeHandle<RotationSpeedComp> rotationSpeedCompTypeHandle;

        public float deltaTime;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
            /*
             * �����е�һ������ʵ����н��õĲ�ѯ���ʱ��useEnableMask����Ϊtrue�����û�в�ѯ�������ʵ��IEnableComponent�������ǿ��Լ���useEnabledMask��ʼ��Ϊfalse�����ǣ���Ӵ˱��������һ���ܺõ��������Է��Ժ����˸��Ĳ�ѯ��������͡�
             */
            Assert.IsFalse(useEnabledMask);
            var transforms = chunk.GetNativeArray(ref transformTypeHandle);
            var rotationSpeeds = chunk.GetNativeArray(ref rotationSpeedCompTypeHandle);
            for (int i = 0; i < chunk.Count; i++) {
                transforms[i] = transforms[i].RotateZ(rotationSpeeds[i].RadiansPerSecond * deltaTime);
            }
        }
    }
}