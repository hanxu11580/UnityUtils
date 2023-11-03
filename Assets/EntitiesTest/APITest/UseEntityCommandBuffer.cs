using Unity.Entities;


public struct UseEntityCommandBufferSystemData_1 : IComponentData {
    public float value;
}
public struct UseEntityCommandBufferSystemData_2 : IComponentData {
    public float value;
}

public partial class UseEntityCommandBufferSystem : SystemBase {
    protected override void OnUpdate() {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        Entities
            .ForEach((Entity e, in UseEntityCommandBufferSystemData_1 data) => {
                if(data.value > 0) {
                    ecb.AddComponent<UseEntityCommandBufferSystemData_2>(e);
                }
            })
            .Schedule();

        Dependency.Complete();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        // 并行作业
        EntityCommandBuffer.ParallelWriter parallelWriter = ecb.AsParallelWriter();
    }
}
