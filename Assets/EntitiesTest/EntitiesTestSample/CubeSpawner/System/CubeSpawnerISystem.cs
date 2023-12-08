using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

namespace EntitiesTest.CubeSpawner {
    public partial struct CubeSpawnerISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var job = new CubeSpawnerIJob {
                ECB = ecb
            };
            // ���߳�����ִ�У����ܲ�ܶ�
            // Schedule��ScheduleParallel����ָ�ɶ��̲߳���ִ�У����ھֲ������ķ������ǿɶ�����д��������������ݣ�Schedule�ǿɶ���д��ScheduleParallel��ֻ��������������������������


            // state.Dependency.Complete();
            // ��䲻�ӣ��ͻ��������ı���
            /*
             * InvalidOperationException: The previously scheduled job MovingJob writes to the ComponentTypeHandle<EntitiesTest.CubeSpawner.RndComponent> MovingJob.JobData.__TypeHandle.__EntitiesTest_CubeSpawner_MovingAspect_RW_AspectTypeHandle.MovingAspect_rndCompCAc. You are trying to schedule a new job CubeSpawnerIJob, which writes to the same ComponentTypeHandle<EntitiesTest.CubeSpawner.RndComponent> (via CubeSpawnerIJob.JobData.__TypeHandle.__EntitiesTest_CubeSpawner_CubeSpawnerAspect_RW_AspectTypeHandle.CubeSpawnerAspect_rndCAc). To guarantee safety, you must include MovingJob as a dependency of the newly scheduled job.
             */
            state.Dependency.Complete();
            job.Run();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    readonly partial struct CubeSpawnerAspect : IAspect {
        private readonly RefRW<CubeSpawner> spawner;
        private readonly RefRW<RndComponent> rnd;
        public void Spawn(EntityCommandBuffer ecb) {
            var count = spawner.ValueRO.spawnCount;
            for (int i = 0; i < count; i++) {
                spawner.ValueRW.spawnCount--;
                Entity instance = ecb.Instantiate(spawner.ValueRO.prefab);

                var seed = rnd.ValueRW.random.NextUInt();
                var random = new Unity.Mathematics.Random(seed);
                var init_direction = random.NextFloat3Direction();
                ecb.SetComponent(instance, new RndComponent { seed = seed, random = random, randomVector = init_direction });
                ecb.SetComponent(instance, LocalTransform.FromPosition(spawner.ValueRO.spawnPosition));
            }
        }
    }


    partial struct CubeSpawnerIJob : IJobEntity {
        public EntityCommandBuffer ECB;
        
        private void Execute(CubeSpawnerAspect aspect) {
            aspect.Spawn(ECB);
        }
    }
}